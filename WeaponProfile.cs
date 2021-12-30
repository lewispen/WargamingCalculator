namespace WargamingCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class WeaponProfile
    {
        public string Attacks { get; set; }

        public int Strength { get; set; }

        public int WeaponSkill { get; set; }

        public int BallisticSkill { get; set; }

        public int HitModifier { get; set; }
    
        public int WoundModifier { get; set; }  
        
        public int NumberOf { get; set; }

        public bool IsRangedWeapon { get; set; }

        public bool MortalSixes { get; set; }

        public bool ReRollOnes { get; set; }

        public bool ReRollWoundOnes { get; set; }

        public bool ReRollFailedHits { get; set; }

        public bool ReRollFailedWounds { get; set; }

        public int AP { get; set; }

        public string Damage { get; set; }

        public bool AlreadyReRolledHits { get; set; }

        public bool AlreadyReRolledWounds { get; set; }

        public int StrengthModifier { get; set; }
    }
}
