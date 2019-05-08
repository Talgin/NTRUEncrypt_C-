#region Directives
using System;
using Test.Tests.Arith;
using Test.Tests.Encode;
using Test.Tests.Encrypt;
using Test.Tests.Polynomial;
using VTDev.Libraries.CEXEngine.Crypto.Cipher.Asymmetric.Encrypt.NTRU;
using System.Runtime.InteropServices;
using System.Diagnostics;
using VTDev.Libraries.CEXEngine.Crypto.Cipher.Asymmetric.Interfaces;
using VTDev.Libraries.CEXEngine.Tools;
using VTDev.Libraries.CEXEngine.Crypto.Prng;
using Test.Tests;
using VTDev.Libraries.CEXEngine.Crypto;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace Test
{
    /// <summary>
    /// Original NTRUEncrypt paper: http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.25.8422&rep=rep1&type=pdf
    /// Follow-up NTRUEncrypt paper: http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.64.6834&rep=rep1&type=pdf
    /// Original NTRUSign paper: http://www.math.brown.edu/~jpipher/NTRUSign_RSA.pdf
    /// Follow-up NTRUSign paper: http://grouper.ieee.org/groups/1363/WorkingGroup/presentations/NTRUSignParams-2005-08.pdf
    /// NTRU articles (technical and mathematical): http://www.securityinnovation.com/security-lab/crypto.html
    /// Jeffrey Hoffstein et al: An Introduction to Mathematical Cryptography, Springer-Verlag, ISBN 978-0-387-77993-5 
    /// EESS: http://grouper.ieee.org/groups/1363/lattPK/submissions/EESS1v2.pdf
    /// </summary>
    class Program:System.Windows.Forms.Form
    {
        const int CYCLE_COUNT = 100;
        private Button btn_Validation;
        private TextBox tb_Console;
        private Button btn_Speed;
        private Button btn_Looping;
        const string CON_TITLE = "NTRU> ";
        Program() // ADD THIS CONSTRUCTOR
        {
            InitializeComponent();
        }

        private void Program_Load(object sender, EventArgs e)
        {
            // header
            tb_Console.Text = "**********************************************\r\n";
            tb_Console.Text += "* NTRU Encrypt Testing in WinForms C#        *\r\n";
            tb_Console.Text += "*                                            *\r\n";
            tb_Console.Text += "* Date:      May 8, 2019                     *\r\n";
            tb_Console.Text += "**********************************************\r\n";
            tb_Console.Text += "\r\n";            

            if (Debugger.IsAttached)
            {
                tb_Console.Text += "Вы используете режим Отладки! Скомпилированная сборка будет работать быстрее.. \r\n";
                tb_Console.Text += "\r\n";
            }

            tb_Console.Text += CON_TITLE + "Запустить валидационные тесты? Нажмите кнопку справа, чтобы запустить тесты.. \r\n";           
            tb_Console.Text += "\r\n";
        }
        private void btn_Validation_Click(object sender, EventArgs e)
        {
            // math
            tb_Console.Text += "******ТЕСТ BIGINTEGER MATH FUNCTIONS******\r\n";
            RunTest(new BigIntEuclideanTest());
            RunTest(new IntEuclideanTest());
            RunTest(new SchönhageStrassenTest());/**/

            // polynomials
            tb_Console.Text += "******ТЕСТ POLYNOMINAL FUNCTIONS******\r\n";
            RunTest(new BigDecimalPolynomialTest());
            RunTest(new BigIntPolynomialTest());
            RunTest(new DenseTernaryPolynomialTest());
            RunTest(new IntegerPolynomialTest());
            RunTest(new LongPolynomial2Test());
            RunTest(new LongPolynomial5Test());
            RunTest(new ProductFormPolynomialTest());
            RunTest(new SparseTernaryPolynomialTest());
            tb_Console.Text += "\r\n";/**/

            // utils
            tb_Console.Text += "******ТЕСТ ARRAY ENCODERS******\r\n";
            RunTest(new ArrayEncoderTest());
            tb_Console.Text += "\r\n";/**/

            // encrypt
            tb_Console.Text += "******ТЕСТ ENCRYPTION ENGINE******\r\n";
            RunTest(new BitStringTest());
            RunTest(new NtruKeyPairTest());
            RunTest(new NtruEncryptTest());
            RunTest(new NtruKeyTest());
            RunTest(new NtruParametersTest());
            RunTest(new IndexGeneratorTest());
            RunTest(new PBPRngTest());
            tb_Console.Text += "\r\n";/**/

            tb_Console.Text += "Валидационные тесты завершены!\r\n";

            tb_Console.Text += "\r\n";
            tb_Console.Text += CON_TITLE + "Запустить тесты скорости? Нажмите кнопку справа, чтобы запустить тесты..\r\n";
            tb_Console.Text += "\r\n";
        }

        private void btn_Speed_Click(object sender, EventArgs e)
        {
            EncryptionSpeed(CYCLE_COUNT);
            DecryptionSpeed(CYCLE_COUNT);
            KeyGenSpeed(CYCLE_COUNT);
            tb_Console.Text += "Тесты скорости завершены!\r\n";
            tb_Console.Text += "\r\n";

            tb_Console.Text += "\r\n";
            tb_Console.Text += CON_TITLE + "Запустить циклические тесты? Нажмите кнпоку справа, чтобы запустить..\r\n";
            tb_Console.Text += "\r\n";
        }

        private void btn_Looping_Click(object sender, EventArgs e)
        {
            tb_Console.Text += "\r\n";
            tb_Console.Text += "******Циклические тесты: Генерация ключей/Шифрование/Дешифрование и верификация******\r\n";
            tb_Console.Text += string.Format("Тестирование {0} полных циклов, выводит на неудачные попытки..", CYCLE_COUNT) + "\r\n";
            try
            {
                CycleTest(CYCLE_COUNT);
            }
            catch (Exception ex)
            {
                tb_Console.Text += "!Циклические тесты провалены! " + ex.Message + "\r\n";
            }
            tb_Console.Text += "\r\n";
            tb_Console.Text += CON_TITLE + "Все тесты завершены, спасибо за внимание..\r\n";
        }
        #region Main
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new Program());
        }

        private void RunTest(ITest Test)
        {
            try
            {
                Test.Progress -= OnTestProgress;
                Test.Progress += new EventHandler<TestEventArgs>(OnTestProgress);
                tb_Console.Text += Test.Description + "\r\n";
                tb_Console.Text += Test.Test() + "\r\n";
                tb_Console.Text += "\r\n";
            }
            catch (Exception Ex)
            {
                tb_Console.Text += "!Произошла ошибка!\r\n";
                tb_Console.Text += Ex.Message + "\r\n";
                tb_Console.Text += "\r\n";
                tb_Console.Text += ">Продолжить тесты? Нажмите кнопку теста справа для продолжения..\r\n";
            }
        }

        private void OnTestProgress(object sender, TestEventArgs e)
        {
            tb_Console.Text += e.Message + "\r\n";
        }
        #endregion

        #region Timing Tests
        void CycleTest(int Iterations)
        {
            Stopwatch runTimer = new Stopwatch();
            runTimer.Start();
            for (int i = 0; i < Iterations; i++)
                FullCycle();
            runTimer.Stop();

            double elapsed = runTimer.Elapsed.TotalMilliseconds;
            tb_Console.Text += string.Format("{0} циклов завершены за: {1} ms", Iterations, elapsed) + "\r\n";
            tb_Console.Text += string.Format("Среднее время цикла: {0} ms", elapsed / Iterations) + "\r\n";
            tb_Console.Text += "\r\n";
        }

        void DecryptionSpeed(int Iterations)
        {
            tb_Console.Text += string.Format("******Циклические тесты на дешифрование: Тестирование {0} Итерации******", Iterations) + "\r\n";

            tb_Console.Text += "Тестирование количества дешифровании с использованием множества параметров APR2011439FAST.\r\n";
            double elapsed = Decrypt(Iterations, NTRUParamSets.APR2011439FAST);
            tb_Console.Text += string.Format("{0} циклов дешифрования завершены за: {1} ms", Iterations, elapsed) + "\r\n";
            tb_Console.Text += string.Format("{0} итерации было проведено в средн. за {1} ms.", Iterations, elapsed / Iterations) + "\r\n";
            tb_Console.Text += string.Format("Скорость дешифрования {0} в секунду", (int)(1000.0 / (elapsed / Iterations))) + "\r\n";
            tb_Console.Text += "\r\n";

            tb_Console.Text += "Тестирование количества дешифровании с использованием множества параметров APR2011743FAST.\r\n";
            elapsed = Decrypt(Iterations, NTRUParamSets.APR2011743FAST);
            tb_Console.Text += string.Format("{0} циклов дешифрования завершены за: {1} ms", Iterations, elapsed) + "\r\n";
            tb_Console.Text += string.Format("{0} итерации было проведено в среднем за {1} ms.", Iterations, elapsed / Iterations) + "\r\n";
            tb_Console.Text += string.Format("Скорость дешифрования {0} в секунду", (int)(1000.0 / (elapsed / Iterations))) + "\r\n";
            tb_Console.Text += "\r\n";
        }

        static void EncryptionSpeed(int Iterations)
        {
            Program prg = new Program();
            prg.tb_Console.Text += string.Format("******Циклические тесты на шифрование: Тестирование {0} Итерации******", Iterations) + "\r\n";

            prg.tb_Console.Text += "Тестирование количества шифровании с использованием множества параметров APR2011439FAST.\r\n";
            double elapsed = Encrypt(Iterations, NTRUParamSets.APR2011439FAST);
            prg.tb_Console.Text += string.Format("{0} циклов дешифрования завершены за: {1} ms", Iterations, elapsed) + "\r\n";
            prg.tb_Console.Text += string.Format("{0} итерации было проведено в среднем за {1} ms.", Iterations, elapsed / Iterations) + "\r\n";
            prg.tb_Console.Text += string.Format("Скорость шифрования {0} в секунду", (int)(1000.0 / (elapsed / Iterations))) + "\r\n";
            prg.tb_Console.Text += "\r\n";

            prg.tb_Console.Text += "Тестирование количества шифровании с использованием множества параметров APR2011743FAST.\r\n";
            elapsed = Encrypt(Iterations, NTRUParamSets.APR2011743FAST);
            prg.tb_Console.Text += string.Format("{0} циклов дешифрования завершены за: {1} ms", Iterations, elapsed) + "\r\n";
            prg.tb_Console.Text += string.Format("{0} итерации было проведено в среднем за {1} ms.", Iterations, elapsed / Iterations) + "\r\n";
            prg.tb_Console.Text += string.Format("Скорость шифрования {0} в секунду", (int)(1000.0 / (elapsed / Iterations))) + "\r\n";
            prg.tb_Console.Text += "\r\n";
        }

        static void FullCycle()
        {
            NTRUParameters mpar = NTRUParamSets.APR2011439FAST; //APR2011743FAST
            NTRUKeyGenerator mkgen = new NTRUKeyGenerator(mpar);
            IAsymmetricKeyPair akp = mkgen.GenerateKeyPair();
            byte[] enc;

            using (NTRUEncrypt mpe = new NTRUEncrypt(mpar))
            {
                mpe.Initialize(akp.PublicKey);

                byte[] data = new byte[mpe.MaxPlainText];
                enc = mpe.Encrypt(data);
                mpe.Initialize(akp);
                byte[] dec = mpe.Decrypt(enc);

                if (!Compare.AreEqual(dec, data))
                    throw new Exception("Тест шифрования: отказ дешифрования!");
            }
        }

        static void KeyGenSpeed(int Iterations)
        {
            Program prg = new Program();
            prg.tb_Console.Text += string.Format("Среднее время генерации ключей за {0} проходов:", Iterations) + "\r\n";
            Stopwatch runTimer = new Stopwatch();
            double elapsed;

            elapsed = KeyGenerator(Iterations, NTRUParamSets.APR2011439FAST);
            prg.tb_Console.Text += string.Format("APR2011439FAST: в среднем {0} ms", elapsed / Iterations, Iterations) + "\r\n";
            prg.tb_Console.Text += string.Format("{0} ключей создано за: {1} ms", Iterations, elapsed) + "\r\n";
            prg.tb_Console.Text += string.Format("Скорость создания ключей: {0} ключей в секунду", (int)(1000.0 / (elapsed / Iterations))) + "\r\n";
            prg.tb_Console.Text += "\r\n";

            elapsed = KeyGenerator(Iterations, NTRUParamSets.APR2011743FAST);
            prg.tb_Console.Text += string.Format("APR2011743FAST: в среднем {0} ms", elapsed / Iterations, Iterations) + "\r\n";
            prg.tb_Console.Text += string.Format("{0} ключей создано за: {1} ms", Iterations, elapsed) + "\r\n";
            prg.tb_Console.Text += string.Format("Скорость создания ключей: {0} ключей в секунду", (int)(1000.0 / (elapsed / Iterations))) + "\r\n";
            prg.tb_Console.Text += "\r\n";

            Iterations = 4;
            prg.tb_Console.Text += string.Format("Тестирование каждого ключа за {0} проходов:", Iterations) + "\r\n";
            prg.tb_Console.Text += "\r\n";

            foreach (int p in Enum.GetValues(typeof(NTRUParamSets.NTRUParamNames)))
            {
                NTRUParameters param = NTRUParamSets.FromName((NTRUParamSets.NTRUParamNames)p);
                elapsed = KeyGenerator(Iterations, param);
                prg.tb_Console.Text += string.Format(Enum.GetName(typeof(NTRUParamSets.NTRUParamNames), p) + ": в среднем за {0} ms", elapsed / Iterations, Iterations) + "\r\n";
                prg.tb_Console.Text += string.Format("{0} ключей создано за: {1} ms", Iterations, elapsed) + "\r\n";
                prg.tb_Console.Text += "\r\n";
            }

            prg.tb_Console.Text += "\r\n";
        }

        static double KeyGenerator(int Iterations, NTRUParameters Param)
        {
            NTRUKeyGenerator mkgen = new NTRUKeyGenerator(Param);
            IAsymmetricKeyPair akp;
            Stopwatch runTimer = new Stopwatch();

            runTimer.Start();
            for (int i = 0; i < Iterations; i++)
                akp = mkgen.GenerateKeyPair();
            runTimer.Stop();

            return runTimer.Elapsed.TotalMilliseconds;
        }

        static double Decrypt(int Iterations, NTRUParameters Param)
        {
            NTRUKeyGenerator mkgen = new NTRUKeyGenerator(Param);
            IAsymmetricKeyPair akp = mkgen.GenerateKeyPair();
            byte[] ptext = new CSPRng().GetBytes(64);
            byte[] rtext = new byte[64];
            byte[] ctext;
            Stopwatch runTimer = new Stopwatch();

            using (NTRUEncrypt mpe = new NTRUEncrypt(Param))
            {
                mpe.Initialize(akp.PublicKey);
                ctext = mpe.Encrypt(ptext);
                mpe.Initialize(akp);

                runTimer.Start();
                for (int i = 0; i < Iterations; i++)
                    rtext = mpe.Decrypt(ctext);
                runTimer.Stop();
            }

            return runTimer.Elapsed.TotalMilliseconds;
        }

        static double Encrypt(int Iterations, NTRUParameters Param)
        {
            NTRUKeyGenerator mkgen = new NTRUKeyGenerator(Param);
            IAsymmetricKeyPair akp = mkgen.GenerateKeyPair();
            byte[] ptext = new CSPRng().GetBytes(64);
            byte[] ctext;
            Stopwatch runTimer = new Stopwatch();

            using (NTRUEncrypt mpe = new NTRUEncrypt(Param))
            {
                mpe.Initialize(akp.PublicKey);

                runTimer.Start();
                for (int i = 0; i < Iterations; i++)
                    ctext = mpe.Encrypt(ptext);
                runTimer.Stop();
            }

            return runTimer.Elapsed.TotalMilliseconds;
        }
        #endregion

        private void InitializeComponent()
        {
            this.btn_Validation = new System.Windows.Forms.Button();
            this.tb_Console = new System.Windows.Forms.TextBox();
            this.btn_Speed = new System.Windows.Forms.Button();
            this.btn_Looping = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Validation
            // 
            this.btn_Validation.Location = new System.Drawing.Point(636, 12);
            this.btn_Validation.Name = "btn_Validation";
            this.btn_Validation.Size = new System.Drawing.Size(122, 36);
            this.btn_Validation.TabIndex = 0;
            this.btn_Validation.Text = "Запуск проверочных тестов";
            this.btn_Validation.UseVisualStyleBackColor = true;
            this.btn_Validation.Click += new System.EventHandler(this.btn_Validation_Click);
            // 
            // tb_Console
            // 
            this.tb_Console.Location = new System.Drawing.Point(12, 12);
            this.tb_Console.Multiline = true;
            this.tb_Console.Name = "tb_Console";
            this.tb_Console.ReadOnly = true;
            this.tb_Console.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_Console.Size = new System.Drawing.Size(618, 277);
            this.tb_Console.TabIndex = 1;
            // 
            // btn_Speed
            // 
            this.btn_Speed.Location = new System.Drawing.Point(636, 54);
            this.btn_Speed.Name = "btn_Speed";
            this.btn_Speed.Size = new System.Drawing.Size(122, 36);
            this.btn_Speed.TabIndex = 2;
            this.btn_Speed.Text = "Запуск тестов скорости";
            this.btn_Speed.UseVisualStyleBackColor = true;
            this.btn_Speed.Click += new System.EventHandler(this.btn_Speed_Click);
            // 
            // btn_Looping
            // 
            this.btn_Looping.Location = new System.Drawing.Point(636, 96);
            this.btn_Looping.Name = "btn_Looping";
            this.btn_Looping.Size = new System.Drawing.Size(122, 36);
            this.btn_Looping.TabIndex = 3;
            this.btn_Looping.Text = "Запуск циклических тестов";
            this.btn_Looping.UseVisualStyleBackColor = true;
            this.btn_Looping.Click += new System.EventHandler(this.btn_Looping_Click);
            // 
            // Program
            // 
            this.ClientSize = new System.Drawing.Size(770, 301);
            this.Controls.Add(this.btn_Looping);
            this.Controls.Add(this.btn_Speed);
            this.Controls.Add(this.tb_Console);
            this.Controls.Add(this.btn_Validation);
            this.Name = "Program";
            this.Load += new System.EventHandler(this.Program_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}
