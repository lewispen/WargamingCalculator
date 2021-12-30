namespace WargamingCalculator
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
            Clear();

            rand = new Random();
            List<int> hits = new List<int>();

            if (weaponProfiles != null)
            {
                foreach (WeaponProfile weaponProfile in weaponProfiles)
                {
                    CurrentHits.Clear();
                    CurrentWounds.Clear();
                    CalculateHits(weaponProfile, weaponProfile.Attacks);
                    Hits.AddRange(CurrentHits);
                    CalculateWounds(weaponProfile, CurrentHits.Count);
                    Wounds.AddRange(CurrentWounds);
                    CurrentSaves.Clear();
                    int failedSaves = CalculateSaves(weaponProfile, CurrentWounds);
                    Saves.AddRange(CurrentSaves);
                    if (CurrentCombatantProfile.FeelNoPain > 0)
                    {
                        CalculateFNP(weaponProfile, failedSaves);
                    }
                    PotentialDamageTotal += weaponProfile.Attacks * weaponProfile.Damage;
                    SaveResults();
                }
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
        }

        private void CalculateHits(WeaponProfile weaponProfile, int attacks)
        {
            List<int> failedHits = new List<int>();

            for (int i = 0; i < attacks; i++)
            {
                int hitRoll = rand.Next(1, 7);

                if (hitRoll == 1 && weaponProfile.ReRollOnes && !weaponProfile.AlreadyReRolledHits)
                {
                    hitRoll = rand.Next(1, 7);
                }

                if (hitRoll < 6 && weaponProfile.HitModifier != 0)
                {
                    hitRoll += weaponProfile.HitModifier;
                }

                if (CurrentCombatantProfile.HitModifier != 0)
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

        private void CalculateWounds(WeaponProfile weaponProfile, int hits)
        {
            List<int> failedWounds = new List<int>();
            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    int woundroll = rand.Next(1, 7);

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
                            Damage += weaponProfile.Damage;
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
                            Damage += weaponProfile.Damage;
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
                            Damage += weaponProfile.Damage;
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
                            Damage += weaponProfile.Damage;
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
                            Damage += weaponProfile.Damage;
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
                    CalculateWounds(weaponProfile, failedWounds.Count);
                }
            }
        }

        private int CalculateSaves(WeaponProfile weaponProfile, List<int> wounds)
        {
            int currentSavingThrow = CurrentCombatantProfile.SavingThrow - weaponProfile.AP;
            List<int> failedSavingThrows = new List<int>();

            for (int i = 0; i < wounds.Count; i++)
            {
                int savingThrowRoll = rand.Next(1, 7);

                if (currentSavingThrow > CurrentCombatantProfile.InvulnerableSave && CurrentCombatantProfile.InvulnerableSave > 0)
                {
                    if (savingThrowRoll >= CurrentCombatantProfile.InvulnerableSave)
                    {
                        CurrentSaves.Add(savingThrowRoll);
                        Damage -= weaponProfile.Damage;
                    }
                }
                else if (savingThrowRoll > CurrentCombatantProfile.SavingThrow)
                {
                    CurrentSaves.Add(savingThrowRoll);
                    Damage -= weaponProfile.Damage;
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
                    Damage -= weaponProfile.Damage;
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
