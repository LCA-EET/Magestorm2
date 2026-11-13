using System.Collections.Generic;

namespace Magus
{
    public static class TextFieldRegister
    {
        private static int _nextID = 1;
        private static Dictionary<int, TextField> _register;
        public static void Init()
        {
            _register = new Dictionary<int, TextField>();
        }
        public static int RegisterField(TextField toRegister)
        {
            _nextID++;
            _register.Add(_nextID, toRegister);
            return _nextID;
        }
        public static void DeregisterField(int id)
        {
            _register.Remove(id);
        }

        public static bool FieldHasFocus()
        {
            foreach (TextField field in _register.Values)
            {
                if(field != null)
                {
                    if (field.IsFocused)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
