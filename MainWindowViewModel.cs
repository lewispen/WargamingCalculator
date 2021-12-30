namespace WargamingCalculator
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows.Input;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Initilisation
        private List<WeaponProfile> weaponProfiles;

        private WeaponProfile currentWeaponProfile;

        private CombatantProfile currentCombatantProfile;

        private ICommand addWeaponProfileCommand;

        private ICommand calculateOutputCommand;

        private ICommand resetCommand;

        private List<int> hits;

        private int hitCount;

        private List<int> currentHits;

        private List<int> wounds;

        private int woundCount;

        private List<int> currentWounds;

        private List<int> saves;

        private List<int> currentSaves;

        private int saveCount;

        private int damage;

        private int potentialDamageTotal;

        private Random rand;

        private int mortalWounds;

        private float averageDamage;

        private float averageWounds;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public MainWindowViewModel()
        {
            WeaponProfiles = new List<WeaponProfile>();
            CurrentWeaponProfile = new WeaponProfile();
            CurrentCombatantProfile = new CombatantProfile();

            Hits = new List<int>();
            CurrentHits = new List<int>();
            Saves = new List<int>();
            CurrentSaves = new List<int>();
            Wounds = new List<int>();
            CurrentWounds = new List<int>();

            HitCount = Hits.Count;
            SaveCount = Saves.Count;
            WoundCount = Wounds.Count;
            MortalWounds = 0;
            AverageDamage = 0;
            AverageWounds = 0;

            addWeaponProfileCommand = new RelayCommand(AddWeaponProfileToList);
            calculateOutputCommand = new RelayCommand(CalculateOutput);
            resetCommand = new RelayCommand(ClearAll);
        }

        #region Commands
        public ICommand AddWeaponProfileCommand
        {
            get
            {
                return addWeaponProfileCommand;
            }
            set
            {
                addWeaponProfileCommand = value;
            }
        }

        public ICommand CalculateOutputCommand
        {
            get
            {
                return calculateOutputCommand;
            }
            set
            {
                calculateOutputCommand = value;
            }
        }

        public ICommand ResetCommand
        {
            get
            {
                return resetCommand;
            }
            set
            {
                resetCommand = value;
            }
        }
        #endregion

        #region Getter/Setters
        public WeaponProfile CurrentWeaponProfile
        {
            get { return currentWeaponProfile; }
            set { currentWeaponProfile = value; }
        }

        public List<WeaponProfile> WeaponProfiles
        {
            get { return weaponProfiles; }
            set { weaponProfiles = value; }
        }

        public CombatantProfile CurrentCombatantProfile
        {
            get { return currentCombatantProfile; }
            set { currentCombatantProfile = value; }
        }

        public List<int> Hits
        {
            get { return hits; }
            set
            {
                if (hits != value)
                {
                    hits = value;
                    OnPropertyChanged("Hits");
                }
            }
        }

        public List<int> CurrentHits
        {
            get { return currentHits; }
            set { currentHits = value; }
        }

        public List<int> CurrentWounds
        {
            get { return currentWounds; }
            set { currentWounds = value; }
        }

        public List<int> CurrentSaves
        {
            get { return currentSaves; }
            set { currentSaves = value; }
        }

        public int HitCount
        {
            get { return hitCount; }
            set
            {
                if (hitCount != value)
                {
                    hitCount = value;
                    OnPropertyChanged("HitCount");
                }
            }
        }

        public List<int> Saves
        {
            get { return saves; }
            set
            {
                if (saves != value)
                {
                    saves = value;
                    OnPropertyChanged("Saves");
                }
            }
        }

        public int SaveCount
        {
            get { return saveCount; }
            set
            {
                if (saveCount != value)
                {
                    saveCount = value;
                    OnPropertyChanged("SaveCount");
                }
            }
        }

        public List<int> Wounds
        {
            get { return wounds; }
            set
            {
                if (wounds != value)
                {
                    wounds = value;
                    OnPropertyChanged("Wounds");
                }
            }
        }

        public int WoundCount
        {
            get { return woundCount; }
            set
            {
                if (woundCount != value)
                {
                    woundCount = value;
                    OnPropertyChanged("WoundCount");
                }
            }
        }

        public int Damage
        {
            get { return damage; }
            set
            {
                if (damage != value)
                {
                    damage = value;
                    OnPropertyChanged("Damage");
                }
            }
        }

        public float AverageDamage
        {
            get { return averageDamage; }
            set
            {
                if (averageDamage != value)
                {
                    averageDamage = value;
                    OnPropertyChanged("AverageDamage");
                }
            }
        }

        public float AverageWounds
        {
            get { return averageWounds; }
            set
            {
                if (averageWounds != value)
                {
                    averageWounds = value;
                    OnPropertyChanged("AverageWounds");
                }
            }
        }

        public int PotentialDamageTotal
        {
            get { return potentialDamageTotal; }
            set
            {
                if (potentialDamageTotal != value)
                {
                    potentialDamageTotal = value;
                    OnPropertyChanged("PotentialDamageTotal");
                }
            }
        }

        public int MortalWounds
        {
            get { return mortalWounds; }
            set
            {
                if (mortalWounds != value)
                {
                    mortalWounds = value;
                    OnPropertyChanged("MortalWounds");
                }
            }
        }

        #endregion

        public void AddWeaponProfileToList(object obj)
        {
            for (int i = 0; i < CurrentWeaponProfile.NumberOf; i++)
            {
                WeaponProfiles.Add(CurrentWeaponProfile);
            }
        }

        public void CalculateOutput(object obj)
        { 
            rand = new Random();
            List<int> hits = new List<int>();
            AverageDamage = 0;
            AverageWounds = 0;

            if (weaponProfiles != null)
            {
                for (int i = 0; i < 100; i++)
                {
                    AverageDamage += Damage;
                    AverageWounds += Wounds.Count();
                    Clear();
                    foreach (WeaponProfile weaponProfile in weaponProfiles)
                    {
                        CurrentHits.Clear();
                        CurrentWounds.Clear();
                        int weaponProfileAttacks = 0;
                        List<string> attacks = weaponProfile.Attacks.Split('d').ToList();

                        if (attacks.Count == 1)
                        {
                            weaponProfileAttacks = int.Parse(weaponProfile.Attacks);
                        }
                        else
                        {
                            int highAttackBracket = int.Parse(attacks[1]) + 1;
                            weaponProfileAttacks = rand.Next(1, highAttackBracket);
                        }

                        int weaponProfileDamage = 0;
                        List<string> damage = weaponProfile.Damage.Split('d').ToList();

                        if(damage.Count == 1)
                        {
                            weaponProfileDamage = int.Parse(weaponProfile.Damage);
                        }
                        else
                        {
                            int highDamageBracket = int.Parse(damage[1]) + 1;
                            weaponProfileDamage = rand.Next(1, highDamageBracket);
                        }

                        if (weaponProfileDamage > 1 && CurrentCombatantProfile.DamageModifier != 0)
                        {
                            weaponProfileDamage -= 1;
                        }

                        CalculateHits(weaponProfile, weaponProfileAttacks);
                        Hits.AddRange(CurrentHits);
                        CalculateWounds(weaponProfile, CurrentHits.Count, weaponProfileDamage);
                        Wounds.AddRange(CurrentWounds);
                        CurrentSaves.Clear();
                        int failedSaves = CalculateSaves(weaponProfile, CurrentWounds, weaponProfileDamage);
                        Saves.AddRange(CurrentSaves);
                        if (CurrentCombatantProfile.FeelNoPain > 0)
                        {
                            CalculateFNP(weaponProfile, failedSaves);
                        }

                        PotentialDamageTotal += CurrentHits.Count * weaponProfileDamage;



                        SaveResults();
                    }
                }
                AverageDamage = AverageDamage / 100;
                AverageWounds = AverageWounds / 100;
            }
        }

        private void Clear()
        {
            Hits = new List<int>();
            HitCount = 0;
            Saves = new List<int>();
            SaveCount = 0;
            Wounds = new List<int>();
            WoundCount = 0;
            Damage = 0;
            MortalWounds = 0;
            PotentialDamageTotal = 0;
        }

        private void ClearAll(object obj)
        {
            Clear();
            WeaponProfiles.Clear();
            AverageDamage = 0;
            AverageWounds = 0;
        }

        private void CalculateHits(WeaponProfile weaponProfile, int attacks)
        {
            List<int> failedHits = new List<int>();
            bool negativeHitModifierApplied = false;
            for (int i = 0; i < attacks; i++)
            {
                int hitRoll = rand.Next(1, 7);

                if (hitRoll == 1 && weaponProfile.ReRollOnes && !weaponProfile.AlreadyReRolledHits)
                {
                    hitRoll = rand.Next(1, 7);
                }

                if (hitRoll < 6 && weaponProfile.HitModifier > 0)
                {
                    hitRoll += weaponProfile.HitModifier;
                }
                else if(hitRoll > 1 && weaponProfile.HitModifier < 0)
                {
                    hitRoll += weaponProfile.HitModifier;
                    negativeHitModifierApplied = true;
                }

                if (CurrentCombatantProfile.HitModifier != 0 && !negativeHitModifierApplied)
                {
                    hitRoll -= CurrentCombatantProfile.HitModifier;
                }

                if (weaponProfile.IsRangedWeapon && weaponProfile.BallisticSkill <= hitRoll)
                {
                    CurrentHits.Add(hitRoll);
                }
                else if (!weaponProfile.IsRangedWeapon && weaponProfile.WeaponSkill <= hitRoll)
                {
                    CurrentHits.Add(hitRoll);
                }
                else
                {
                    failedHits.Add(hitRoll);
                }
            }

            if (failedHits.Count > 0 && weaponProfile.ReRollFailedHits && !weaponProfile.AlreadyReRolledHits)
            {
                weaponProfile.AlreadyReRolledHits = true;
                CalculateHits(weaponProfile, failedHits.Count);
            }
        }

        private void CalculateWounds(WeaponProfile weaponProfile, int hits, int damage)
        {
            List<int> failedWounds = new List<int>();
            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    int woundroll = rand.Next(1, 7);

                    if (woundroll == 1 && weaponProfile.ReRollWoundOnes && !weaponProfile.AlreadyReRolledWounds)
                    {
                        woundroll = rand.Next(1, 7);
                    }

                    if (woundroll == 6 && weaponProfile.MortalSixes)
                    {
                        MortalWounds += 1;
                    }

                    if (woundroll < 6 && weaponProfile.WoundModifier != 0)
                    {
                        woundroll += weaponProfile.WoundModifier;
                    }

                    if ((weaponProfile.Strength / (CurrentCombatantProfile.Toughness - CurrentCombatantProfile.ToughnessModifier)) >= 2)
                    {
                        if (woundroll >= 2)
                        {
                            CurrentWounds.Add(woundroll);
                            Damage += damage;
                        }
                        else
                        {
                            failedWounds.Add(woundroll);
                        }
                    }
                    else if (weaponProfile.Strength > (CurrentCombatantProfile.Toughness - CurrentCombatantProfile.ToughnessModifier))
                    {
                        if (woundroll >= 3)
                        {
                            CurrentWounds.Add(woundroll);
                            Damage += damage;
                        }
                        else
                        {
                            failedWounds.Add(woundroll);
                        }
                    }
                    else if (weaponProfile.Strength == (CurrentCombatantProfile.Toughness - CurrentCombatantProfile.ToughnessModifier))
                    {
                        if (woundroll >= 4)
                        {
                            CurrentWounds.Add(woundroll);
                            Damage += damage;
                        }
                        else
                        {
                            failedWounds.Add(woundroll);
                        }
                    }
                    else if (weaponProfile.Strength < (CurrentCombatantProfile.Toughness - CurrentCombatantProfile.ToughnessModifier))
                    {
                        if (woundroll >= 5)
                        {
                            CurrentWounds.Add(woundroll);
                            Damage += damage;
                        }
                        else
                        {
                            failedWounds.Add(woundroll);
                        }
                    }
                    else if ((weaponProfile.Strength / (CurrentCombatantProfile.Toughness - CurrentCombatantProfile.ToughnessModifier)) >= 0.5)
                    {
                        if (woundroll >= 6)
                        {
                            CurrentWounds.Add(woundroll);
                            Damage += damage;
                        }
                        else
                        {
                            failedWounds.Add(woundroll);
                        }
                    }
                }

                if (failedWounds.Count > 0 && weaponProfile.ReRollFailedWounds && !weaponProfile.AlreadyReRolledWounds)
                {
                    weaponProfile.AlreadyReRolledWounds = true;
                    CalculateWounds(weaponProfile, failedWounds.Count, damage);
                }
            }
        }

        private int CalculateSaves(WeaponProfile weaponProfile, List<int> wounds, int damage)
        {
            int currentSavingThrow = CurrentCombatantProfile.SavingThrow + weaponProfile.AP;
            List<int> failedSavingThrows = new List<int>();

            for (int i = 0; i < wounds.Count; i++)
            {
                int savingThrowRoll = rand.Next(1, 7);

                if (currentSavingThrow > CurrentCombatantProfile.InvulnerableSave && CurrentCombatantProfile.InvulnerableSave > 0)
                {
                    if (savingThrowRoll >= CurrentCombatantProfile.InvulnerableSave)
                    {
                        CurrentSaves.Add(savingThrowRoll);
                        Damage -= damage;
                    }
                    else
                    {
                        failedSavingThrows.Add(savingThrowRoll);
                    }
                }
                else if (savingThrowRoll > currentSavingThrow)
                {
                    CurrentSaves.Add(savingThrowRoll);
                    Damage -= damage;
                }
                else
                {
                    failedSavingThrows.Add(savingThrowRoll);
                }
            }
            return failedSavingThrows.Count;
        }

        private void CalculateFNP(WeaponProfile weaponProfile, int failedSaves)
        {
            for (int i = 0; i < failedSaves; i++)
            {
                int FNPRoll = rand.Next(1, 7);
                if (FNPRoll >= CurrentCombatantProfile.FeelNoPain)
                {
                    Saves.Add(FNPRoll);
                    Damage -= damage;
                }
            }
        }

        private void SaveResults()
        {
            HitCount = Hits.Count;
            SaveCount = Saves.Count;
            WoundCount = Wounds.Count;
            Damage = Damage;
            PotentialDamageTotal = PotentialDamageTotal;
            MortalWounds = MortalWounds;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
