import java.net.DatagramPacket;
import java.nio.charset.StandardCharsets;
import java.util.Base64;
import java.util.ArrayList;
import java.util.Collection;

public class PregamePacketProcessor extends UDPProcessor
{
    private final RegisteredThread _asyncThread;
    private final AsyncPGPP _asyncProcessor;
    private int _accountID;
    public PregamePacketProcessor(int port){
        super(port, ServerParams.Encryption);
        GameServer.PGP = this;
        _asyncProcessor = new AsyncPGPP("AsyncPGPP, port " + port, this);
        _asyncThread = new RegisteredThread(_asyncProcessor);
        _asyncThread.start();
    }
    @Override
    public void TerminateProcessor(){
        super.TerminateProcessor();
        _asyncThread.interrupt();
    }
    @Override protected boolean PreProcess(UDPTransmission received)
    {
        super.PreProcess(received);
        _remote = LoggedInClient();
        if(_remote != null){
            _accountID =(int)_remote.ObjectID();
        }
        else{
            _remote = new RemoteClient(received);
        }
        return true;
    }
    @Override
    protected boolean ProcessPacket(UDPTransmission received) {
        PreProcess(received);
        switch (_opCode) {
            case Pregame_Receive.CreateCharacter, Pregame_Receive.DeleteCharacter,
                 Pregame_Receive.NameCheck, Pregame_Receive.UpdateAppearance,
                 Pregame_Receive.UpdateSkills, Pregame_Receive.UpdateSkillsAndSlotting,
                 Pregame_Receive.CreateAccount, Pregame_Receive.LogIn ->
                _asyncProcessor.EnqueueRequest(new AsyncPGPPRequest(_decrypted, _opCode, _remote, _accountID));
            case Pregame_Receive.LogOut ->
                RemoteClientManager.ClientLoggedOut(_accountID);
            case Pregame_Receive.SubscribeToMatches ->
                HandleMatchSubscribePacket(true);
            case Pregame_Receive.UnsubscribeFromMatches ->
                HandleMatchSubscribePacket(false);
            case Pregame_Receive.CreateMatch ->
                MatchManager.RequestMatchCreation(_accountID, _decrypted[5], _decrypted[6], _decrypted[7], _decrypted[8]);
            case Pregame_Receive.DeleteMatch ->
                MatchManager.DeleteMatch(_accountID, _remote);
            case Pregame_Receive.RequestLevelsList ->
                EnqueueForSend(Packets.LevelListPacket(), _remote);
            case Pregame_Receive.RequestMatchDetails ->
                HandleMatchDetailsPacket();
            case Pregame_Receive.JoinMatch ->
                HandleJoinMatchPacket(_decrypted[5], _decrypted[6]);
            case Pregame_Receive.RequestMatchList ->
                MatchManager.SendMatchListToClient(_remote);
            case Pregame_Receive.RequestMatchScore ->
                SendMatchScore(_decrypted[5], _remote);
            case Pregame_Receive.RequestSpellData ->
                HandleSpellDataRequest();
            case Pregame_Receive.RequestDisciplineData ->
                EnqueueForSend(DisciplineManager.GetDisciplinePacket(), _remote);
            case Pregame_Receive.RequestCharacterClassData ->
                EnqueueForSend(CharacterClassManager.ClientDataPacket(), _remote);
            case Pregame_Receive.OTPReply ->
                HandleOTPReply();
        }
        return true;
    }
    private void HandleSpellDataRequest(){
        Collection<byte[]> packets = SpellManager.GetSpellDataPackets();
        for(byte[] packet : packets){
            EnqueueForSend(packet, _remote);
        }
    }
    private void HandleOTPReply(){
        int accountID = ByteUtils.ExtractInt(_decrypted, 1);
        int otp = ByteUtils.ExtractInt(_decrypted, 5);
        boolean result = Database.ActivateAccount(accountID, otp);
        EnqueueForSend(Packets.ActivationResultPacket(result?(byte)1:(byte)0), _remote);
    }
    private void HandleJoinMatchPacket(byte matchID, byte teamID)
    {
        Match toJoin = MatchManager.GetMatch(matchID);
        if(toJoin != null){
            if(toJoin.HasRoomForAnotherPlayer()){
                RemoteClient remote = RemoteClientManager.GetClient(_accountID);
                remote.UnsubscribeFromMatches();
                toJoin.JoinMatch(remote, teamID);
            }
            else{
                EnqueueForSend(Packets.MatchIsFullPacket(), _remote);
            }
        }
    }
    public void HandleMatchDetailsPacket(){
        byte matchID = _decrypted[5];
        Match match = MatchManager.GetMatch(matchID);
        if(match != null){
            if(match.IsOptionEnabled(ControlCodes.MatchOptions_AntiStack)){
                byte teamToJoin = match.GetASTeam();
                HandleJoinMatchPacket(matchID, teamToJoin);
            }
            else{
                EnqueueForSend(Packets.MatchDetailsPacket(match), _remote);
            }
        }
    }
    public void HandleMatchSubscribePacket(boolean subscribe){
        RemoteClient rc = MatchManager.Subscribe(_accountID, subscribe, ByteUtils.ExtractInt(_decrypted, 5));
        if(subscribe){
            byte priorMatchID = _decrypted[9];
            if(_decrypted[10] == 1){ // qm
                HandleJoinMatchPacket(MatchManager.GetQMID(), (byte)0);
            }
            else if (priorMatchID != 0){
                SendMatchScore(priorMatchID, rc);
            }
        }
    }
    private void SendMatchScore(byte matchID, RemoteClient rc){
        byte[] toEncrypt= MatchManager.GetScoreBytes(matchID);
        EnqueueForSend(Packets.MatchScoresPacket(Pregame_Send.MatchScore, toEncrypt == null ? new byte[2]:toEncrypt), rc);
    }
}
