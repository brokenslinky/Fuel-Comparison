using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Fuel_Comparison
{
    public partial class Form1 : Form
    {
        public class Molecule
        {
            public int carbon = 0;
            public int hydrogen = 0;
            public int oxygen = 0;
            public double enthalpy;
            public double entropy;
            public double density;
            public double molar_mass()
            {
                return 12.0107 * carbon + 1.00794 * hydrogen + 15.999 * oxygen;
            }
            public string chemical_formula()
            {
                string formula = string.Empty;
                if (carbon != 0)
                    formula += "C" + carbon.ToString();
                if (hydrogen != 0)
                    formula += "H" + hydrogen.ToString();
                if (oxygen != 0)
                    formula += "O" + oxygen.ToString();
                return formula;
            }
        }

        #region gas properties
        Molecule O2 = new Molecule
        {
            oxygen = 2,
            enthalpy = 0.0,
            entropy = 205.07
        };
        Molecule H2O = new Molecule
        {
            enthalpy = -241.83,
            entropy = 188.84
        };
        Molecule CO = new Molecule
        {
            enthalpy = -110.525,
            entropy = 197.674
        };
        Molecule CO2 = new Molecule
        {
            enthalpy = -393.509,
            entropy = 213.74
        };

        double air_molar_mass = 28.97;
        #endregion

        public class Reaction
        {
            public int fuel_in;
            public int O2_in;
            public int H2O_out;
            public int CO_out;
            public int CO2_out;
            public double air_fuel_ratio;
            public double energy_per_fuel;
            public double energy_per_O2;
        }

        public Reaction rich_combustion(Molecule fuel)
        {
            Reaction reaction = new Reaction
            {
                fuel_in = 4,
                O2_in = 2 * fuel.carbon + fuel.hydrogen - 2 * fuel.oxygen,
                H2O_out = 2 * fuel.hydrogen,
                CO_out = 4 * fuel.carbon,
                CO2_out = 0
            };
            double intake_temperature = Convert.ToDouble(intake_text_box.Text);
            double exhaust_temperature = Convert.ToDouble(exhaust_text_box.Text);
            double enthalpy_change = reaction.H2O_out * H2O.enthalpy + reaction.CO_out * CO.enthalpy - 
                reaction.fuel_in * fuel.enthalpy - reaction.O2_in * O2.enthalpy;
            double entropy_in = reaction.fuel_in * fuel.entropy + reaction.O2_in * O2.entropy;
            double entropy_out = reaction.H2O_out * H2O.entropy + reaction.CO2_out * CO2.entropy + reaction.CO_out * 
                CO.entropy;
            double energy = -enthalpy_change + entropy_out * exhaust_temperature + entropy_in * intake_temperature - 
                2.0 * entropy_out * intake_temperature;
            reaction.energy_per_fuel = (energy / reaction.fuel_in) * (Convert.ToDouble(density_text_box.Text) / 
                fuel.molar_mass());
            reaction.energy_per_O2 = energy / reaction.O2_in;
            reaction.air_fuel_ratio = (Convert.ToDouble(reaction.O2_in) / Convert.ToDouble(reaction.fuel_in)) * (air_molar_mass / fuel.molar_mass()) /
                0.2095;
            while (reaction.O2_in % 2 == 0 && reaction.H2O_out % 2 == 0 && reaction.CO_out % 2 == 0)
            {
                reaction.fuel_in /= 2;
                reaction.O2_in /= 2;
                reaction.H2O_out /= 2;
                reaction.CO_out /= 2;
            }
            return reaction;
        }

        public Reaction lean_combustion(Molecule fuel)
        {
            Reaction reaction = new Reaction
            {
                fuel_in = 4,
                O2_in = 4 * fuel.carbon + fuel.hydrogen - 2 * fuel.oxygen,
                H2O_out = 2 * fuel.hydrogen,
                CO_out = 0,
                CO2_out = 4 * fuel.carbon
            };
            double intake_temperature = Convert.ToDouble(intake_text_box.Text);
            double exhaust_temperature = Convert.ToDouble(exhaust_text_box.Text);
            double enthalpy_change = reaction.H2O_out * H2O.enthalpy + reaction.CO2_out * CO2.enthalpy - 
                reaction.fuel_in * fuel.enthalpy - reaction.O2_in * O2.enthalpy;
            double entropy_in = reaction.fuel_in * fuel.entropy + reaction.O2_in * O2.entropy;
            double entropy_out = reaction.H2O_out * H2O.entropy + reaction.CO2_out * CO2.entropy + reaction.CO_out * 
                CO.entropy;
            double energy = -enthalpy_change + entropy_out * exhaust_temperature + entropy_in * intake_temperature - 
                2.0 * entropy_out * intake_temperature;
            reaction.energy_per_fuel = (energy / reaction.fuel_in) * (Convert.ToDouble(density_text_box.Text) / 
                fuel.molar_mass());
            reaction.energy_per_O2 = energy / reaction.O2_in;
            reaction.air_fuel_ratio = (Convert.ToDouble(reaction.O2_in) / Convert.ToDouble(reaction.fuel_in)) * (air_molar_mass / fuel.molar_mass()) / 
                0.2095;
            while (reaction.O2_in % 2 == 0 && reaction.H2O_out % 2 == 0 && reaction.CO2_out % 2 == 0)
            {
                reaction.fuel_in /= 2;
                reaction.O2_in /= 2;
                reaction.H2O_out /= 2;
                reaction.CO2_out /= 2;
            }
            return reaction;
        }

        public string[] get_fuel_list()
        {
            StreamReader reader = new StreamReader(System.Environment.CurrentDirectory + @"\..\..\Fuel Library.txt");
            List<string> name_list = new List<string>();
            bool name_line = true;
            string line = string.Empty;
            while ((line = reader.ReadLine()) != null)
            {
                if (name_line && line != string.Empty)
                {
                    name_list.Add(line);
                    name_line = false;
                }
                if (line == string.Empty)
                    name_line = true;
            }
            reader.Close();
            string[] fuel_names = new string[name_list.Count];
            for (int index = 0; index < name_list.Count; index++)
                fuel_names[index] = name_list[index];
            return fuel_names;
        }

        public Form1()
        {
            InitializeComponent();
            intake_text_box.Text = "300";
            exhaust_text_box.Text = "1000";
            fuel1_dropdown.Items.AddRange(get_fuel_list());
            fuel1_dropdown.SelectedIndex = 0;
            populate_text_boxes(get_fuel_properties(fuel1_dropdown.SelectedItem.ToString()), 1);
            fuel2_dropdown.Items.AddRange(get_fuel_list());
            fuel2_dropdown.SelectedIndex = 1;
            populate_text_boxes(get_fuel_properties(fuel2_dropdown.SelectedItem.ToString()), 2);
        }

        public Molecule get_fuel_properties(string fuel_name)
        {
            Molecule fuel = new Molecule();
            StreamReader reader = new StreamReader(System.Environment.CurrentDirectory + @"\..\..\Fuel Library.txt");
            string line = string.Empty;
            string property = string.Empty;
            while ((line = reader.ReadLine()) != fuel_name)
                continue;
            line = reader.ReadLine();
            bool C = false; bool H = false; bool O = false;
            string word = string.Empty;
            foreach (char c in line)
            {
                if (c == 'C')
                {
                    C = true;
                    if (H)
                    {
                        if (word == string.Empty)
                            fuel.hydrogen = 1;
                        else
                            fuel.hydrogen = Convert.ToInt32(word);
                        H = false;
                    }
                    if (O)
                    {
                        if (word == string.Empty)
                            fuel.oxygen = 1;
                        else
                            fuel.oxygen = Convert.ToInt32(word);
                        O = false;
                    }
                    word = string.Empty;
                }
                else if (c == 'H')
                {
                    H = true;
                    if (C)
                    {
                        if (word == string.Empty)
                            fuel.carbon = 1;
                        else
                            fuel.carbon = Convert.ToInt32(word);
                        C = false;
                    }
                    if (O)
                    {
                        if (word == string.Empty)
                            fuel.oxygen = 1;
                        else
                            fuel.oxygen = Convert.ToInt32(word);
                        O = false;
                    }
                    word = string.Empty;
                }
                else if (c == 'O')
                {
                    O = true;
                    if (C)
                    {
                        if (word == string.Empty)
                            fuel.carbon = 1;
                        else
                            fuel.carbon = Convert.ToInt32(word);
                        C = false;
                    }
                    if (H)
                    {
                        if (word == string.Empty)
                            fuel.hydrogen = 1;
                        else
                            fuel.hydrogen = Convert.ToInt32(word);
                        H = false;
                    }
                    word = string.Empty;
                }
                else
                    word += c;
            }
            if (C)
            {
                if (word == string.Empty)
                    fuel.carbon = 1;
                else
                    fuel.carbon = Convert.ToInt32(word);
                C = false;
            }
            if (H)
            {
                if (word == string.Empty)
                    fuel.hydrogen = 1;
                else
                    fuel.hydrogen = Convert.ToInt32(word);
                H = false;
            }
            if (O)
            {
                if (word == string.Empty)
                    fuel.oxygen = 1;
                else
                    fuel.oxygen = Convert.ToInt32(word);
                O = false;
            }
            while ((line = reader.ReadLine()) != string.Empty && line != null)
            {
                word = string.Empty;
                foreach (char c in line)
                {
                    if (c == '=')
                    {
                        word.Trim();
                        property = word;
                        word = string.Empty;
                    }
                    else
                    {
                        word += c;
                    }
                }
                word.Trim();
                if (property.Contains("nthalpy"))
                    fuel.enthalpy = Convert.ToDouble(word);
                if (property.Contains("ntropy"))
                    fuel.entropy = Convert.ToDouble(word);
                if (property.Contains("ensity"))
                    fuel.density = Convert.ToDouble(word);
            }
            word.Trim();
            if (property.Contains("nthalpy"))
                fuel.enthalpy = Convert.ToDouble(word);
            if (property.Contains("ntropy"))
                fuel.entropy = Convert.ToDouble(word);
            if (property.Contains("ensity"))
                fuel.density = Convert.ToDouble(word);
            return fuel;
        }

        public void populate_text_boxes(Molecule fuel, int fuel_number)
        {
            if (fuel_number == 1)
            {
                carbon_text_box.Text = fuel.carbon.ToString();
                hydrogen_text_box.Text = fuel.hydrogen.ToString();
                oxygen_text_box.Text = fuel.oxygen.ToString();
                enthalpy_text_box.Text = fuel.enthalpy.ToString();
                entropy_text_box.Text = fuel.entropy.ToString();
                density_text_box.Text = fuel.density.ToString();
            }
            if (fuel_number == 2)
            {
                carbon2_text_box.Text = fuel.carbon.ToString();
                hydrogen2_text_box.Text = fuel.hydrogen.ToString();
                oxygen2_text_box.Text = fuel.oxygen.ToString();
                enthalpy2_text_box.Text = fuel.enthalpy.ToString();
                entropy2_text_box.Text = fuel.entropy.ToString();
                density2_text_box.Text = fuel.density.ToString();
            }
        }

        private void fuel1_dropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            populate_text_boxes(get_fuel_properties(fuel1_dropdown.SelectedItem.ToString()), 1);
        }

        private void fuel2_dropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            populate_text_boxes(get_fuel_properties(fuel2_dropdown.SelectedItem.ToString()), 2);
        }

        private void solve_button_Click(object sender, EventArgs e)
        {
            Molecule fuel1 = new Molecule
            {
                carbon = Convert.ToInt32(carbon_text_box.Text),
                hydrogen = Convert.ToInt32(hydrogen_text_box.Text),
                oxygen = Convert.ToInt32(oxygen_text_box.Text),
                enthalpy = Convert.ToDouble(enthalpy_text_box.Text),
                entropy = Convert.ToDouble(entropy_text_box.Text)
            };
            Molecule fuel2 = new Molecule
            {
                carbon = Convert.ToInt32(carbon2_text_box.Text),
                hydrogen = Convert.ToInt32(hydrogen2_text_box.Text),
                oxygen = Convert.ToInt32(oxygen2_text_box.Text),
                enthalpy = Convert.ToDouble(enthalpy2_text_box.Text),
                entropy = Convert.ToDouble(entropy2_text_box.Text)
            };

            #region run lean and rich reactions for both fuels
            Reaction lean_reaction1 = lean_combustion(fuel1);
            Reaction rich_reaction1 = rich_combustion(fuel1);
            Reaction lean_reaction2 = lean_combustion(fuel2);
            Reaction rich_reaction2 = rich_combustion(fuel2);
            #endregion
            #region display each reaction
            lean_reaction1_text_box.Text = lean_reaction1.fuel_in.ToString() + fuel1.chemical_formula() + " + " + lean_reaction1.O2_in.ToString() + "O2 -> " + 
                lean_reaction1.H2O_out.ToString() + "H2O + " + lean_reaction1.CO2_out.ToString() + "CO2";
            lean_reaction2_text_box.Text = lean_reaction2.fuel_in.ToString() + fuel2.chemical_formula() + " + " + lean_reaction2.O2_in.ToString() + "O2 -> " + 
                lean_reaction2.H2O_out.ToString() + "H2O + " + lean_reaction2.CO2_out.ToString() + "CO2";
            rich_reaction1_text_box.Text = rich_reaction1.fuel_in.ToString() + fuel1.chemical_formula() + " + " + rich_reaction1.O2_in.ToString() + "O2 -> " + 
                rich_reaction1.H2O_out.ToString() + "H2O + " + rich_reaction1.CO_out.ToString() + "CO";
            rich_reaction2_text_box.Text = rich_reaction2.fuel_in.ToString() + fuel2.chemical_formula() + " + " + rich_reaction2.O2_in.ToString() + "O2 -> " + 
                rich_reaction2.H2O_out.ToString() + "H2O + " + rich_reaction2.CO_out.ToString() + "CO";
            #endregion
            #region display air:fuel ratios for each reaction
            lean_AFR_text_box1.Text = lean_reaction1.air_fuel_ratio.ToString("N1") + ":1";
            lean_AFR_text_box2.Text = lean_reaction2.air_fuel_ratio.ToString("N1") + ":1";
            rich_AFR_text_box1.Text = rich_reaction1.air_fuel_ratio.ToString("N1") + ":1     λ = " + 
                (rich_reaction1.air_fuel_ratio / lean_reaction1.air_fuel_ratio).ToString("N2");
            rich_AFR_text_box2.Text = rich_reaction2.air_fuel_ratio.ToString("N1") + ":1     λ = " + 
                (rich_reaction2.air_fuel_ratio / lean_reaction2.air_fuel_ratio).ToString("N2");
            #endregion
            #region display energy per fuel (economy) for each reaction
            lean_energy_per_fuel_text_box.Text = (lean_reaction1.energy_per_fuel).ToString("N0") + " kJ/cc";
            lean_energy_per_fuel_text_box2.Text = (lean_reaction2.energy_per_fuel).ToString("N0") + " kJ/cc";
            lean_economy_ratio.Text = (lean_reaction2.energy_per_fuel / lean_reaction1.energy_per_fuel).ToString("N3");
            rich_energy_per_fuel_text_box.Text = (rich_reaction1.energy_per_fuel).ToString("N0") + " kJ/cc";
            rich_energy_per_fuel_text_box2.Text = (rich_reaction2.energy_per_fuel).ToString("N0") + " kJ/cc";
            rich_economy_ratio.Text = (rich_reaction2.energy_per_fuel / rich_reaction1.energy_per_fuel).ToString("N3");
            #endregion
            #region display energy per O2 (power) for each reaction
            lean_energy_per_O2_text_box.Text = lean_reaction1.energy_per_O2.ToString("N0") + " kJ/molO2";
            lean_energy_per_O2_text_box2.Text = lean_reaction2.energy_per_O2.ToString("N0") + " kJ/molO2";
            lean_power_ratio.Text = (lean_reaction2.energy_per_O2 / lean_reaction1.energy_per_O2).ToString("N3");
            rich_energy_per_O2_text_box.Text = rich_reaction1.energy_per_O2.ToString("N0") + " kJ/molO2";
            rich_energy_per_O2_text_box2.Text = rich_reaction2.energy_per_O2.ToString("N0") + " kJ/molO2";
            rich_power_ratio.Text = (rich_reaction2.energy_per_O2 / rich_reaction1.energy_per_O2).ToString("N3");
            #endregion
        }
        
    }
}
