using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            public double molar_mass()
            {
                return 12.0107 * carbon + 1.00794 * hydrogen + 15.999 * oxygen;
            }
        }

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
            double energy = -enthalpy_change + (entropy_out * exhaust_temperature + entropy_in * 
                intake_temperature - 2.0 * entropy_out * intake_temperature) / 1000.0;
            reaction.energy_per_fuel = (energy / reaction.fuel_in) * (Convert.ToDouble(density_text_box.Text) / 
                fuel.molar_mass());
            reaction.energy_per_O2 = energy / reaction.O2_in;
            reaction.air_fuel_ratio = (reaction.O2_in / reaction.fuel_in) * (air_molar_mass / fuel.molar_mass()) /
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
            double energy = -enthalpy_change + (entropy_out * exhaust_temperature + entropy_in * 
                intake_temperature - 2.0 * entropy_out * intake_temperature) / 1000.0;
            reaction.energy_per_fuel = (energy / reaction.fuel_in) * (Convert.ToDouble(density_text_box.Text) / 
                fuel.molar_mass());
            reaction.energy_per_O2 = energy / reaction.O2_in;
            reaction.air_fuel_ratio = (reaction.O2_in / reaction.fuel_in) * (air_molar_mass / fuel.molar_mass()) / 
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

        public Form1()
        {
            InitializeComponent();
            carbon_text_box.Text = "8";
            hydrogen_text_box.Text = "18";
            oxygen_text_box.Text = "0";
            enthaply_text_box.Text = "-249.95";
            entropy_text_box.Text = "361.2";
            density_text_box.Text = "0.703";
            carbon2_text_box.Text = "2";
            hydrogen2_text_box.Text = "6";
            oxygen2_text_box.Text = "1";
            enthalpy2_text_box.Text = "-277.38";
            entropy2_text_box.Text = "159.9";
            density2_text_box.Text = "0.788";
            intake_text_box.Text = "300";
            exhaust_text_box.Text = "1000";
        }

        private void solve_button_Click(object sender, EventArgs e)
        {
            Molecule fuel1 = new Molecule
            {
                carbon = Convert.ToInt32(carbon_text_box.Text),
                hydrogen = Convert.ToInt32(hydrogen_text_box.Text),
                oxygen = Convert.ToInt32(oxygen_text_box.Text),
                enthalpy = Convert.ToDouble(enthaply_text_box.Text),
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
            lean_reaction1_text_box.Text = lean_reaction1.fuel_in.ToString() + "C" + fuel1.carbon.ToString() + 
                "H" + fuel1.hydrogen.ToString() + "O" + fuel1.oxygen.ToString() + " + " + 
                lean_reaction1.O2_in.ToString() + "O2 -> " + lean_reaction1.H2O_out.ToString() + "H2O + " + 
                lean_reaction1.CO2_out.ToString() + "CO2";
            lean_reaction2_text_box.Text = lean_reaction2.fuel_in.ToString() + "C" + fuel2.carbon.ToString() + 
                "H" + fuel2.hydrogen.ToString() + "O" + fuel2.oxygen.ToString() + " + " + 
                lean_reaction2.O2_in.ToString() + "O2 -> " + lean_reaction2.H2O_out.ToString() + "H2O + " + 
                lean_reaction2.CO2_out.ToString() + "CO2";
            rich_reaction1_text_box.Text = rich_reaction1.fuel_in.ToString() + "C" + fuel1.carbon.ToString() + 
                "H" + fuel1.hydrogen.ToString() + "O" + fuel1.oxygen.ToString() + " + " + 
                rich_reaction1.O2_in.ToString() + "O2 -> " + rich_reaction1.H2O_out.ToString() + "H2O + " + 
                rich_reaction1.CO_out.ToString() + "CO";
            rich_reaction2_text_box.Text = rich_reaction2.fuel_in.ToString() + "C" + fuel2.carbon.ToString() + 
                "H" + fuel2.hydrogen.ToString() + "O" + fuel2.oxygen.ToString() + " + " + 
                rich_reaction2.O2_in.ToString() + "O2 -> " + rich_reaction2.H2O_out.ToString() + "H2O + " + 
                rich_reaction2.CO_out.ToString() + "CO";
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
