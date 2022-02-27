using System.IO;

using static Database;

public class File_Manager
{
    public static string save_directory = Directory.GetCurrentDirectory() + "\\SavesOld\\";

    public static bool File_Save()
    {
        StreamWriter sw = new StreamWriter($"{ save_directory }File { (char)('A' + data.slot) }.txt");

        if (sw != null)
        {
            sw.Write(data.slot);
            sw.WriteLine(data.difficulty);

            sw.WriteLine($"{ data.total_money.ToString("0.#") }");
            sw.WriteLine($"{ data.currency.ToString("0.#") }");

            for (int i = 0; i < data.b_data.Length; i++)
            {
                sw.WriteLine($"{ data.b_data[i].count.ToString("0.#") }");
                sw.WriteLine($"{ data.b_data[i].price.ToString("0.#") }");
                sw.WriteLine($"{ data.b_data[i].value.ToString("0.#") }");
                sw.WriteLine($"{ data.b_data[i].click_power_amplifier.ToString("0.#") }");
                sw.WriteLine($"{ data.b_data[i].bps.ToString("0.#") }");
                sw.WriteLine(data.b_data[i].cp_amp_toggle);
                sw.WriteLine(data.b_data[i].available);
            }

            sw.WriteLine();

            for (int i = 0; i < data.u_statuses.Length; i++)
            {
                UnityEngine.Debug.Log($"Upgrade Status #{i} : {data.u_statuses[i]}");
                sw.Write($"{ data.u_statuses[i] }");
            }
                

            sw.WriteLine();

            sw.WriteLine(data.time_played);
            sw.WriteLine(data.time_started);

            sw.WriteLine();

            for (int i = 0; i < Achievements_Data.achievements.Length; i++)
            {
                sw.WriteLine(Achievements_Data.achievements[i].progress);
                sw.WriteLine(Achievements_Data.achievements[i].status);
                sw.WriteLine(Achievements_Data.achievements[i].date_earned.ToBinary());
            }

            sw.WriteLine();

            for (int i = 0; i < Achievements_Data.pending_acheivements.Count; i++)
            {
                sw.WriteLine(Achievements_Data.pending_acheivements[i].id - 500);
                sw.WriteLine(Achievements_Data.pending_acheivements[i].progress);
            }

            sw.WriteLine(".");
            sw.Close();

            UnityEngine.Debug.Log("Save complete");

            return true;
        }
        return false;
    }

    public static bool File_Load(int slot)
    {
        if (File.Exists($"{save_directory}File {(char)('A' + slot)}.txt"))
        {
            data = new Data();

            StreamReader sr = new StreamReader($"{save_directory}File {(char)('A' + slot)}.txt");

            Achievements_Data.Init_Achievements();
            Achievements_Data.pending_acheivements.Clear();

            // sr.Read() gets the char value of the read number, which is not the value we want until we subtract it by the char offset.
            data.slot = sr.Read() - 48;
            data.difficulty = sr.Read() - 48;

            sr.ReadLine();

            double.TryParse(sr.ReadLine(), out data.total_money);
            double.TryParse(sr.ReadLine(), out data.currency);

            for (int i = 0; i < data.b_data.Length; i++)
            {
                int.TryParse(sr.ReadLine(), out data.b_data[i].count);
                double.TryParse(sr.ReadLine(), out data.b_data[i].price);
                double.TryParse(sr.ReadLine(), out data.b_data[i].value);
                double.TryParse(sr.ReadLine(), out data.b_data[i].click_power_amplifier);
                double.TryParse(sr.ReadLine(), out data.b_data[i].bps);
                bool.TryParse(sr.ReadLine(), out data.b_data[i].cp_amp_toggle);
                bool.TryParse(sr.ReadLine(), out data.b_data[i].available);
            }

            sr.ReadLine();

            for (int i = 0; i < data.u_statuses.Length; i++)
            {
                data.u_statuses[i] = sr.Read() - 48;
            }
                

            sr.ReadLine();

            long.TryParse(sr.ReadLine(), out data.time_played);
            long.TryParse(sr.ReadLine(), out data.time_started);

            sr.ReadLine();

            for (int i = 0; i < Achievements_Data.achievements.Length; i++)
            {
                double.TryParse(sr.ReadLine(), out Achievements_Data.achievements[i].progress);
                int.TryParse(sr.ReadLine(), out Achievements_Data.achievements[i].status);
                long.TryParse(sr.ReadLine(), out long date_earned);
                if (date_earned != System.DateTime.FromBinary(0).ToBinary())
                    Achievements_Data.achievements[i].date_earned = System.DateTime.FromBinary(date_earned);
            }

            sr.ReadLine();

            int counter = 0;
            while (int.TryParse(sr.ReadLine(), out int a_id))
                if (Achievements_Data.pending_acheivements != null)
                {
                    Achievements_Data.pending_acheivements.Add(Achievements_Data.achievements[a_id]);
                    double.TryParse(sr.ReadLine(), out Achievements_Data.pending_acheivements[counter++].progress);
                }

            sr.Close();
            return true;
        }
        return false;
    }
    public static void New_File(int difficulty, int slot)
    {
        data = new Data
        {
            slot = slot,
            difficulty = difficulty,
            time_started = System.DateTime.Now.ToBinary()
        };

        // Set base building values
        data.b_data[0].price = 15;
        data.b_data[0].bps = 0.1f;

        data.b_data[1].price = 85;
        data.b_data[1].bps = 1;

        data.b_data[2].price = 700;
        data.b_data[2].bps = 6;

        data.b_data[3].price = 5000;
        data.b_data[3].bps = 20;

        data.b_data[4].price = 25000;
        data.b_data[4].bps = 100;

        data.b_data[5].price = 200000;
        data.b_data[5].bps = 500;

        data.b_data[6].price = 420690;
        data.b_data[6].bps = 1500;

        data.b_data[7].price = 1431100;
        data.b_data[7].bps = 3400;

        data.b_data[8].price = 5000000;
        data.b_data[8].bps = 10000;

        data.b_data[9].price = 20000000;
        data.b_data[9].bps = 25000;

        data.b_data[10].price = 100000000;
        data.b_data[10].bps = 50000;

        data.b_data[11].price = 500000000;
        data.b_data[11].bps = 100000;

        Achievements_Data.Init_Achievements();
        Achievements_Data.Set_First_Acheievements();

        File_Save();
    }
    public static void File_Delete(int slot)
    {
        if(File.Exists($"{ save_directory }//File { (char)('A' + slot) }.txt"))
            File.Delete($"{ save_directory }//File { (char)('A' + slot) }.txt");
    }
}