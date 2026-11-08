using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Foundation_settlement
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        // === ТАБЛИЦА α (из Листа 3) ===
        private static readonly double[] EtaValues =
        {
    1.0, 1.2, 1.4, 1.6, 1.8, 2.2, 2.4, 2.6, 2.8, 3.0, 3.2, 3.4, 3.6, 5.0
};

        private static readonly double[] ZetaValues =
        {
    0.0, 0.4, 0.8, 1.2, 1.6, 2.0, 2.4, 2.8, 3.2, 3.6, 4.0, 4.4, 4.8,
    5.2, 5.6, 6.0, 6.4, 6.8, 7.2, 7.6, 8.0, 8.4, 8.8, 9.2, 9.6, 10.0,
    10.4, 10.8, 11.2, 11.6, 12.0
};

        private static readonly double[,] AlphaTable = new double[,]
        {
    // ζ=0.0  0.4    0.8    1.2    1.6    2.0    2.4    2.8    3.2    3.6    4.0    4.4    4.8    5.2    5.6    6.0    6.4    6.8    7.2    7.6    8.0    8.4    8.8    9.2    9.6   10.0   10.4   10.8   11.2   11.6   12.0
    { 1.000, 0.960, 0.800, 0.606, 0.449, 0.336, 0.257, 0.201, 0.160, 0.131, 0.108, 0.091, 0.077, 0.067, 0.058, 0.051, 0.045, 0.040, 0.036, 0.032, 0.029, 0.026, 0.024, 0.022, 0.020, 0.019, 0.017, 0.016, 0.015, 0.014, 0.013 }, // η=1.0
    { 1.000, 0.966, 0.824, 0.644, 0.491, 0.375, 0.291, 0.231, 0.185, 0.152, 0.127, 0.107, 0.091, 0.079, 0.069, 0.061, 0.054, 0.048, 0.043, 0.038, 0.035, 0.032, 0.029, 0.027, 0.024, 0.023, 0.021, 0.019, 0.018, 0.017, 0.016 }, // η=1.2
    { 1.000, 0.972, 0.848, 0.682, 0.532, 0.414, 0.325, 0.260, 0.210, 0.173, 0.145, 0.123, 0.105, 0.091, 0.079, 0.070, 0.062, 0.055, 0.049, 0.044, 0.040, 0.037, 0.033, 0.031, 0.028, 0.026, 0.024, 0.022, 0.021, 0.020, 0.018 }, // η=1.4
    { 1.000, 0.974, 0.857, 0.700, 0.555, 0.439, 0.350, 0.282, 0.231, 0.191, 0.161, 0.137, 0.118, 0.102, 0.089, 0.079, 0.070, 0.062, 0.056, 0.050, 0.046, 0.042, 0.038, 0.035, 0.032, 0.030, 0.028, 0.026, 0.024, 0.023, 0.021 }, // η=1.6
    { 1.000, 0.975, 0.866, 0.717, 0.578, 0.463, 0.374, 0.304, 0.251, 0.209, 0.176, 0.150, 0.130, 0.113, 0.099, 0.087, 0.077, 0.069, 0.062, 0.056, 0.051, 0.046, 0.042, 0.039, 0.036, 0.033, 0.031, 0.029, 0.027, 0.025, 0.023 }, // η=1.8
    { 1.000, 0.976, 0.871, 0.728, 0.595, 0.484, 0.397, 0.327, 0.273, 0.230, 0.195, 0.168, 0.146, 0.127, 0.112, 0.099, 0.088, 0.079, 0.071, 0.064, 0.059, 0.053, 0.049, 0.045, 0.042, 0.038, 0.036, 0.033, 0.031, 0.029, 0.027 }, // η=2.2
    { 1.000, 0.976, 0.876, 0.739, 0.612, 0.505, 0.419, 0.349, 0.294, 0.250, 0.214, 0.185, 0.161, 0.141, 0.124, 0.110, 0.099, 0.088, 0.080, 0.072, 0.066, 0.060, 0.055, 0.051, 0.047, 0.043, 0.040, 0.037, 0.035, 0.033, 0.031 }, // η=2.4
    { 1.000, 0.977, 0.877, 0.742, 0.617, 0.512, 0.427, 0.358, 0.303, 0.259, 0.223, 0.194, 0.169, 0.149, 0.131, 0.117, 0.105, 0.094, 0.085, 0.077, 0.071, 0.065, 0.059, 0.055, 0.051, 0.047, 0.043, 0.040, 0.038, 0.036, 0.034 }, // η=2.6
    { 1.000, 0.977, 0.878, 0.744, 0.621, 0.518, 0.434, 0.366, 0.312, 0.268, 0.231, 0.202, 0.177, 0.156, 0.138, 0.123, 0.111, 0.099, 0.090, 0.082, 0.075, 0.069, 0.063, 0.058, 0.054, 0.050, 0.046, 0.043, 0.040, 0.038, 0.036 }, // η=2.8
    { 1.000, 0.977, 0.879, 0.747, 0.625, 0.524, 0.442, 0.375, 0.321, 0.277, 0.240, 0.210, 0.185, 0.163, 0.145, 0.130, 0.117, 0.105, 0.095, 0.087, 0.080, 0.073, 0.067, 0.062, 0.057, 0.053, 0.049, 0.046, 0.043, 0.040, 0.038 }, // η=3.0
    { 1.000, 0.977, 0.879, 0.749, 0.629, 0.530, 0.449, 0.383, 0.329, 0.285, 0.248, 0.218, 0.192, 0.170, 0.152, 0.136, 0.122, 0.110, 0.100, 0.091, 0.084, 0.077, 0.071, 0.065, 0.060, 0.056, 0.052, 0.049, 0.046, 0.042, 0.040 }, // η=3.2
    { 1.000, 0.977, 0.880, 0.751, 0.632, 0.534, 0.455, 0.390, 0.337, 0.294, 0.258, 0.228, 0.202, 0.180, 0.162, 0.146, 0.131, 0.119, 0.109, 0.099, 0.092, 0.084, 0.078, 0.072, 0.067, 0.062, 0.058, 0.054, 0.050, 0.047, 0.045 }, // η=3.4
    { 1.000, 0.977, 0.880, 0.752, 0.634, 0.538, 0.460, 0.397, 0.345, 0.302, 0.267, 0.237, 0.211, 0.189, 0.171, 0.155, 0.140, 0.128, 0.117, 0.107, 0.099, 0.091, 0.085, 0.078, 0.073, 0.068, 0.063, 0.059, 0.055, 0.052, 0.049 }, // η=3.6
    { 1.000, 0.977, 0.881, 0.754, 0.639, 0.545, 0.470, 0.410, 0.360, 0.319, 0.285, 0.255, 0.230, 0.208, 0.189, 0.173, 0.158, 0.145, 0.133, 0.123, 0.113, 0.105, 0.098, 0.091, 0.085, 0.079, 0.074, 0.069, 0.065, 0.061, 0.058 }  // η=5.0
        };
        private double GetAlpha(double eta, double zeta)
        {
            if (zeta < ZetaValues[0]) zeta = ZetaValues[0];
            if (zeta > ZetaValues[ZetaValues.Length - 1]) zeta = ZetaValues[ZetaValues.Length - 1];
            if (eta < EtaValues[0]) eta = EtaValues[0];
            if (eta > EtaValues[EtaValues.Length - 1]) eta = EtaValues[EtaValues.Length - 1];

            int iEta = FindInterval(EtaValues, eta);
            int iZeta = FindInterval(ZetaValues, zeta);

            int iEta2 = Math.Min(iEta + 1, EtaValues.Length - 1);
            int iZeta2 = Math.Min(iZeta + 1, ZetaValues.Length - 1);

            double tEta = (iEta == iEta2) ? 0 : (eta - EtaValues[iEta]) / (EtaValues[iEta2] - EtaValues[iEta]);
            double tZeta = (iZeta == iZeta2) ? 0 : (zeta - ZetaValues[iZeta]) / (ZetaValues[iZeta2] - ZetaValues[iZeta]);

            double a11 = AlphaTable[iEta, iZeta];
            double a12 = AlphaTable[iEta, iZeta2];
            double a21 = AlphaTable[iEta2, iZeta];
            double a22 = AlphaTable[iEta2, iZeta2];

            double a1 = a11 + (a12 - a11) * tZeta;
            double a2 = a21 + (a22 - a21) * tZeta;
            double a = a1 + (a2 - a1) * tEta;

            return Math.Round(a, 3, MidpointRounding.AwayFromZero);
        }

        // === ВСПОМОГАТЕЛЬНЫЙ МЕТОД: ИНДЕКС ИНТЕРВАЛА ===
        private int FindInterval(double[] array, double value)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (value >= array[i] && value <= array[i + 1])
                    return i;
            }
            return array.Length - 1;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // === ВСПОМОГАТЕЛЬНЫЙ МЕТОД ПРОВЕРКИ ===
            bool CheckNotEmpty(TextBox tb, string fieldName)
            {
                if (string.IsNullOrEmpty(tb.Text))
                {
                    MessageBox.Show($"Введите {fieldName}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tb.Focus();
                    return false;
                }
                return true;
            }

            // === ПРОВЕРКА ВСЕХ ПОЛЕЙ ===
            if (!CheckNotEmpty(ШП, "ширину подошвы")) return;
            if (!CheckNotEmpty(ДП, "длину подошвы")) return;
            if (!CheckNotEmpty(ВП, "высоту подошвы")) return;
            if (!CheckNotEmpty(ГП, "глубину подошвы")) return;
            if (!CheckNotEmpty(Нагрузка, "нагрузку")) return;

            if (!CheckNotEmpty(H1, "H1")) return;
            if (!CheckNotEmpty(H2, "H2")) return;
            if (!CheckNotEmpty(H3, "H3")) return;
            if (!CheckNotEmpty(H4, "H4")) return;
            if (!CheckNotEmpty(H5, "H5")) return;

            if (!CheckNotEmpty(y1, "y1")) return;
            if (!CheckNotEmpty(y2, "y2")) return;
            if (!CheckNotEmpty(y3, "y3")) return;
            if (!CheckNotEmpty(y4, "y4")) return;

            if (!CheckNotEmpty(E1, "E1")) return;
            if (!CheckNotEmpty(E2, "E2")) return;
            if (!CheckNotEmpty(E3, "E3")) return;
            if (!CheckNotEmpty(E4, "E4")) return;
            if (!CheckNotEmpty(E5, "E5")) return;

            double GetDouble(TextBox tb, string fieldName, bool allowZero = false)
            {
                string cleaned = tb.Text.Replace(',', '.');
                if (!double.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                {
                    MessageBox.Show($"Некорректное значение в поле {fieldName}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tb.Focus();
                    return double.NaN;
                }

                if (!allowZero && value == 0)
                {
                    MessageBox.Show($"{fieldName}: нулевая геометрия", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tb.Focus();
                    return double.NaN;
                }

                return value;
            }

            // === СЧИТЫВАЕМ ДАННЫЕ ===
            double Ширина = GetDouble(ШП, "Ширина подошвы");
            if (double.IsNaN(Ширина)) return;

            double Длина = GetDouble(ДП, "Длина подошвы");
            if (double.IsNaN(Длина)) return;

            double Высота = GetDouble(ВП, "Высота подошвы");
            if (double.IsNaN(Высота)) return;

            double Глубина = GetDouble(ГП, "Глубина подошвы");
            if (double.IsNaN(Глубина)) return;

            double N = GetDouble(Нагрузка, "Нагрузка");
            if (double.IsNaN(N)) return;

            double Hi1 = GetDouble(H1, "H1", allowZero: true);
            if (double.IsNaN(Hi1)) return;

            double Hi2 = GetDouble(H2, "H2", allowZero: true);
            if (double.IsNaN(Hi2)) return;

            double Hi3 = GetDouble(H3, "H3", allowZero: true);
            if (double.IsNaN(Hi3)) return;

            double Hi4 = GetDouble(H4, "H4", allowZero: true);
            if (double.IsNaN(Hi4)) return;

            double Hi5 = GetDouble(H5, "H5", allowZero: true);
            if (double.IsNaN(Hi5)) return;

            double yi1 = GetDouble(y1, "y1", allowZero: true);
            if (double.IsNaN(yi1)) return;

            double yi2 = GetDouble(y2, "y2", allowZero: true);
            if (double.IsNaN(yi2)) return;

            double yi3 = GetDouble(y3, "y3", allowZero: true);
            if (double.IsNaN(yi3)) return;

            double yi4 = GetDouble(y4, "y4", allowZero: true);
            if (double.IsNaN(yi4)) return;

            double yi5 = GetDouble(y5, "y5", allowZero: true);
            if (double.IsNaN(yi5)) return;

            double Ei1 = GetDouble(E1, "E1", allowZero: true);
            if (double.IsNaN(Ei1)) return;

            double Ei2 = GetDouble(E2, "E2", allowZero: true);
            if (double.IsNaN(Ei2)) return;

            double Ei3 = GetDouble(E3, "E3", allowZero: true);
            if (double.IsNaN(Ei3)) return;

            double Ei4 = GetDouble(E4, "E4", allowZero: true);
            if (double.IsNaN(Ei4)) return;

            double Ei5 = GetDouble(E5, "E5", allowZero: true);
            if (double.IsNaN(Ei5)) return;

            double Hсж = 0.4 * Ширина;
            Толща.Text = Hсж.ToString();

            double zi1 = Hi1 / 2.0;
            double zi2 = Hi1 + Hi2 / 2.0;
            double zi3 = Hi1 + Hi2 + Hi3 / 2.0;
            double zi4 = Hi1 + Hi2 + Hi3 + Hi4 / 2.0;
            double zi5 = Hi1 + Hi2 + Hi3 + Hi4 + Hi5 / 2.0;

            double nu1 = Math.Round(2.0 * zi1 / Ширина, 1, MidpointRounding.AwayFromZero);
            double nu2 = Math.Round(2.0 * zi2 / Ширина, 1, MidpointRounding.AwayFromZero);
            double nu3 = Math.Round(2.0 * zi3 / Ширина, 1, MidpointRounding.AwayFromZero);
            double nu4 = Math.Round(2.0 * zi4 / Ширина, 1, MidpointRounding.AwayFromZero);
            double nu5 = Math.Round(2.0 * zi5 / Ширина, 1, MidpointRounding.AwayFromZero);

            // === η = L/b ===
            double eta = Длина / Ширина;
            // === среднее давление под плитой ===
            double Pср = Math.Round(N / (Длина * Ширина), 1, MidpointRounding.AwayFromZero);

            // === α для каждого слоя ===
            double alpha1 = GetAlpha(eta, nu1);
            double alpha2 = GetAlpha(eta, nu2);
            double alpha3 = GetAlpha(eta, nu3);
            double alpha4 = GetAlpha(eta, nu4);
            double alpha5 = GetAlpha(eta, nu5);

            // === бzp,i= α1*Pср ===
            double бzp1 = Math.Round(alpha1 * Pср, 2, MidpointRounding.AwayFromZero);
            double бzp2 = Math.Round(alpha2 * Pср, 2, MidpointRounding.AwayFromZero);
            double бzp3 = Math.Round(alpha3 * Pср, 2, MidpointRounding.AwayFromZero);
            double бzp4 = Math.Round(alpha4 * Pср, 2, MidpointRounding.AwayFromZero);
            double бzp5 = Math.Round(alpha5 * Pср, 2, MidpointRounding.AwayFromZero);

            // === бzy,i= α1*Pср ===
            double бzy1 = Math.Round(alpha1 * yi1, 2, MidpointRounding.AwayFromZero);
            double бzy2 = Math.Round(alpha2 * yi2, 2, MidpointRounding.AwayFromZero);
            double бzy3 = Math.Round(alpha3 * yi3, 2, MidpointRounding.AwayFromZero);
            double бzy4 = Math.Round(alpha4 * yi4, 2, MidpointRounding.AwayFromZero);
            double бzy5 = Math.Round(alpha5 * yi5, 2, MidpointRounding.AwayFromZero);

            // === 0,8 · 1000 · σzp,i · hi / Ei ===
            // === 0,8 · 1000 · σzy,i · hi / Ei ===

            double Sip1 = 0;
            double Siy1 = 0;

            if (Ei1 > 0)
            {
                Sip1 = Math.Round(0.8 * 1000 * бzp1 * Hi1 / Ei1, 2, MidpointRounding.AwayFromZero);
                Siy1 = Math.Round(0.8 * 1000 * бzy1 * Hi1 / Ei1, 2, MidpointRounding.AwayFromZero);
            }

            double Sip2 = 0;
            double Siy2 = 0;

            if (Ei2 > 0)
            {
                Sip2 = Math.Round(0.8 * 1000 * бzp2 * Hi2 / Ei2, 2, MidpointRounding.AwayFromZero);
                Siy2 = Math.Round(0.8 * 1000 * бzy2 * Hi2 / Ei2, 2, MidpointRounding.AwayFromZero);
            }

            double Sip3 = 0;
            double Siy3 = 0;

            if (Ei3 > 0)
            {
                Sip3 = Math.Round(0.8 * 1000 * бzp3 * Hi3 / Ei3, 2, MidpointRounding.AwayFromZero);
                Siy3 = Math.Round(0.8 * 1000 * бzy3 * Hi3 / Ei3, 2, MidpointRounding.AwayFromZero);
            }

            double Sip4 = 0;
            double Siy4 = 0;

            if (Ei4 > 0)
            {
                Sip4 = Math.Round(0.8 * 1000 * бzp4 * Hi4 / Ei4, 2, MidpointRounding.AwayFromZero);
                Siy4 = Math.Round(0.8 * 1000 * бzy4 * Hi4 / Ei4, 2, MidpointRounding.AwayFromZero);
            }

            double Sip5 = 0;
            double Siy5 = 0;

            if (Ei5 > 0)
            {
                Sip5 = Math.Round(0.8 * 1000 * бzp5 * Hi5 / Ei5, 2, MidpointRounding.AwayFromZero);
                Siy5 = Math.Round(0.8 * 1000 * бzy5 * Hi5 / Ei5, 2, MidpointRounding.AwayFromZero);
            }
            // === S = ΣSip + ΣSiy ===
            double s = (Sip1 + Sip2 + Sip3 + Sip4 + Sip5) + (Siy1 + Siy2 + Siy3 + Siy4 + Siy5);
            Result.Text = s.ToString();

            // === ВЫВОД ===
            string res = "";
            res = "s = β · 1000 · Σ [ (σzp,i · hi / Ei) + (σzγ,i · hi / Ei) ]";
            res += Environment.NewLine + $"s — осадка фундамента (мм); β = 0.8 — коэффициент бокового расширения";
            res += Environment.NewLine + $"1000 — перевод метров в миллиметры";
            res += Environment.NewLine + $"αi — коэффициент затухания дополнительного напряжения (по таблице)";
            res += Environment.NewLine + $"zi — глубина середины i-го слоя от подошвы фундамента (м)";
            res += Environment.NewLine + $"nu (ζ) = 2·zi / b — безразмерная координата глубины для поиска α";
            res += Environment.NewLine + $"b — ширина подошвы фундамента (м)";
            res += Environment.NewLine + $"p = N / (b · L) — среднее давление под подошвой фундамента {Pср} (тс/м²)";
            res += Environment.NewLine + $"σzp,i = αi · p — дополнительное давление от нагрузки, затухающее с глубиной";
            res += Environment.NewLine + $"σzγ,i = αi · yi — напряжение от собственного веса грунта, затухающее с глубиной";
            res += Environment.NewLine + $"hi — толщина i-го слоя (м); Ei — модуль деформации i-го слоя (тс/м²)";
            res += Environment.NewLine + $"yi — удельный вес i-го слоя (тс/м³)";
            res += Environment.NewLine + $"{Name1.Text} | Hi1 = {Hi1} | yi1 = {yi1} | Ei1 = {Ei1} | zi1 = {zi1} | nu1 = {nu1} | α1 = {alpha1} | бzp1 = {бzp1} | бzy1 = {бzy1} | Sip1 = {Sip1} | Siy1 = {Siy1} |";
            res += Environment.NewLine + $"{Name2.Text} | Hi2 = {Hi2} | yi2 = {yi2} | Ei2 = {Ei2} | zi2 = {zi2} | nu2 = {nu2} | α2 = {alpha2} | бzp2 = {бzp2} | бzy2 = {бzy2} | Sip2 = {Sip2} | Siy2 = {Siy2} |";
            res += Environment.NewLine + $"{Name3.Text} | Hi3 = {Hi3} | yi3 = {yi3} | Ei3 = {Ei3} | zi3 = {zi3} | nu3 = {nu3} | α3 = {alpha3} | бzp3 = {бzp3} | бzy3 = {бzy3} | Sip3 = {Sip3} | Siy3 = {Siy3} |";
            res += Environment.NewLine + $"{Name4.Text} | Hi4 = {Hi4} | yi4 = {yi4} | Ei4 = {Ei4} | zi4 = {zi4} | nu4 = {nu4} | α4 = {alpha4} | бzp4 = {бzp4} | бzy4 = {бzy4} | Sip4 = {Sip4} | Siy4 = {Siy4} |";
            res += Environment.NewLine + $"{Name5.Text} | Hi5 = {Hi5} | yi5 = {yi5} | Ei5 = {Ei5} | zi5 = {zi5} | nu5 = {nu5} | α5 = {alpha5} | бzp5 = {бzp5} | бzy5 = {бzy5} | Sip5 = {Sip5} | Siy5 = {Siy5} |";
            res += Environment.NewLine + $"Суммарная осадка составляет {s} мм";

            // === СОХРАНЕНИЕ В ФАЙЛ (в папку с программой) ===
            string path = Path.Combine(Application.StartupPath, $"Осадка_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            File.WriteAllText(path, res, Encoding.UTF8);

            // === ВЫВОД В MESSAGEBOX ===
            MessageBox.Show(res + "\n\n✅ Отчёт сохранён:\n" + path, "Результаты расчёта",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

           


               

