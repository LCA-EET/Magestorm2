import java.util.ArrayList;

public class AntiStack {
    private final Match _owningMatch;

    public AntiStack(Match owningMatch){
        _owningMatch = owningMatch;
    }

    public byte GetTeamToJoin(){
        byte teamToJoin = 0;
        ArrayList<Byte> options = new ArrayList<>();
        for(byte code : MatchTeam.TeamCodes_NonNeutral){
            if(_owningMatch.GetMatchType() == MatchType.DeathMatch){
                if(((DeathMatch)_owningMatch).IsTeamAlive(code)){
                    options.add(code);
                }
            }
            else{
                options.add(code);
            }
        }
        int leastLevel = -1;
        for(byte teamID : options){
            if(leastLevel == -1){
                leastLevel = _owningMatch.GetMatchTeam(teamID).GetTotalLevel();
                teamToJoin = teamID;
            }
            else{
                int totalLevel = _owningMatch.GetMatchTeam(teamID).GetTotalLevel();
                if(totalLevel < leastLevel){
                    leastLevel = totalLevel;
                    teamToJoin = teamID;
                }
            }
        }
        return teamToJoin;
    }
}
