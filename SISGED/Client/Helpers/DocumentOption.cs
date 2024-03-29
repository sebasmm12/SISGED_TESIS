﻿namespace SISGED.Client.Helpers
{
    public class DocumentOption : SelectOption
    {
        public DocumentOption(string label, string value, Roles rol) : base(label, value)
        {
            Rol = rol;
        }

        public Roles Rol { get; set; }


        public override bool Equals(object? obj)
        {
            var other = obj as DocumentOption;
            return other?.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Label;
        }
    }
}
