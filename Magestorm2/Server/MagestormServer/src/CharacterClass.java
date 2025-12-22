public class CharacterClass {
    public static final byte Arcanist = 0;
    public static final byte Cleric = 1;
    public static final byte Magician = 2;
    public static final byte Mentalist = 3;

    private static final byte[] ArcanistSkills = new byte[]
            {
                    ControlCodes.Discipline_ManaLaw,
                    ControlCodes.Discipline_VoidLaw,
                    ControlCodes.Discipline_Sigils,
                    ControlCodes.Discipline_Shielding
            };

    private static final byte[] ClericSkills = new byte[]
            {
                    ControlCodes.Discipline_Barriers,
                    ControlCodes.Discipline_Healing,
                    ControlCodes.Discipline_Smiting,
                    ControlCodes.Discipline_SpiritLaw,
                    ControlCodes.Discipline_Supplication
            };

    private static final byte[] MagicianSkills = new byte[]
            {
                    ControlCodes.Discipline_FireLaw,
                    ControlCodes.Discipline_IceLaw,
                    ControlCodes.Discipline_EarthLaw,
                    ControlCodes.Discipline_Shielding
            };

    private static final byte[] MentalistSkills = new byte[]
            {
                    ControlCodes.Discipline_Brilliance,
                    ControlCodes.Discipline_Psionics,
                    ControlCodes.Discipline_Displacement,
                    ControlCodes.Discipline_Shielding
            };

    public static byte[] GetBaseSkills(byte classID){
        return switch (classID) {
            case CharacterClass.Arcanist -> ArcanistSkills;
            case CharacterClass.Cleric -> ClericSkills;
            case CharacterClass.Magician -> MagicianSkills;
            case CharacterClass.Mentalist -> MentalistSkills;
            default -> null;
        };
    }
    private final byte _classID;

    public CharacterClass(byte id){
        _classID = id;
    }

    public byte GetClass(){
        return _classID;
    }

    public boolean IsArcanist(){
        return _classID == Arcanist;
    }

    public boolean IsCleric(){
        return _classID == Cleric;
    }

    public boolean IsMagician(){
        return _classID == Magician;
    }

    public boolean IsMentalist(){
        return _classID == Mentalist;
    }

    public byte BiasMultiplier(){
        return (byte)(_classID == Magician?2:1);
    }

    public byte ShrineMultiplier(){
        return (byte)(_classID == Cleric?2:1);
    }

    public byte HPMultiplier(){
        return switch(_classID){
            case Cleric -> 6;
            case Magician -> 4;
            default -> 5;
        };
    }



    public String ToString(){
        return switch (_classID) {
            case Arcanist -> "Arcanist";
            case Cleric -> "Cleric";
            case Magician -> "Magician";
            default -> "Mentalist";
        };
    }
}
