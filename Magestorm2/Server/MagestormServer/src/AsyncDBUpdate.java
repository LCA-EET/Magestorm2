import java.sql.Connection;

public class AsyncDBUpdate {
    private final byte _opCode;
    private final MatchCharacter _subject;
    private final byte[] _toProcess;
    private Connection _persistent;
    public AsyncDBUpdate(byte opCode, MatchCharacter subject, byte[] toProcess){
        _opCode = opCode;
        _subject = subject;
        _toProcess = toProcess;
    }

    public void ProcessRequest(Connection persistent){
        _persistent = persistent;
        switch(_opCode){
            case ControlCodes.AsyncDBUpdate_Experience:
                Main.LogDebug("Updating experience for " + _subject.GetCharacterID());
                ExperienceUpdate();
                break;
            case ControlCodes.AsyncDBUpdate_Slotting:
                Main.LogDebug("Updating slotting for " + _subject.GetCharacterID());
                SlottingUpdate();
                break;
        }
    }

    private void ExperienceUpdate(){
        int experience = _subject.GetEndingXP();
        int startingExp = _subject.GetStartingXP();
        if(experience != startingExp){
            int characterID = _subject.GetCharacterID();
            PlayerCharacter pc = _subject.PC();
            pc.UpdateExperience(experience);
            byte currentLevel = pc.GetCharacterLevel();
            byte newLevel = LevelData.DetermineLevel(experience);
            if(newLevel != currentLevel){
                pc.UpdateLevel(newLevel);
            }
            Database.UpdateExperience(characterID, experience, newLevel, _persistent);
        }
    }

    private void SlottingUpdate(){
        byte[] slots = new byte[10];
        System.arraycopy(_toProcess, 2, slots, 0, 10);
        PlayerCharacter pc = _subject.PC();
        pc.UpdateSlottedSpells(slots);
        Database.UpdateSlotting(pc.GetCharacterID(), slots, _persistent);
    }
}
