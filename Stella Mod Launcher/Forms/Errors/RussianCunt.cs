using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;

namespace StellaLauncher.Forms.Errors
{
    public partial class RussianCunt : Form
    {
        private readonly Random _random = new Random();

        public RussianCunt()
        {
            InitializeComponent();
        }

        private void WrongCountry_Shown(object sender, EventArgs e)
        {
            try
            {
                new SoundPlayer { SoundLocation = Path.Combine(Program.AppPath, "data", "sounds", "kurwa.wav") }.Play();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex.ToString());
            }

            Timer randomSizeTimer = new Timer { Interval = 500 };
            randomSizeTimer.Tick += ChangeWindowSize;
            randomSizeTimer.Start();

            Timer closeAppTimer = new Timer { Interval = 34300 };
            closeAppTimer.Tick += (sender1, e1) =>
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(desktopPath, "hello uwu.txt");

                File.WriteAllText(filePath,
                    "Нет, это не вирус. Ваш компьютер полностью исправен. Covid не заразил ваш компьютер. Вы можете быть спокойны.\n\n" +
                    "Однако, если вы действительно хотите запустить программу Stella Mod в России, стоит сначала задуматься о смысле вашего существования.\n" +
                    "Это первый шаг, который вам следует предпринять, тщательно обдумав выборы, которые вы сделали, путь, которым вы прошли, цель и смысл вашего существования, прежде чем приступать к дальнейшим действиям.\n\n" +
                    "После тщательного анализа своей жизни стоит уделить внимание образованию, особенно изучению английского языка.\n" +
                    "Знание этого языка обязательно пригодится в России, а также откроет двери в широкий мир.\n" +
                    "Вы познакомитесь с другими людьми, станете более открытыми к миру. Вы узнаете, что такое интеграция в обществе.\n" +
                    "Ваша жизнь приобретет большую ценность. Это позволит вам перестать общаться только на одном языке, если человек, с которым вы разговариваете, родом из другой страны и не говорит свободно на вашем родном языке.\n\n" +
                    "Еще одной темой для обдумывания является ценность веры в Бога. Вера в Бога имеет глубокие корни и может принести много пользы индивидууму.\n" +
                    "Для многих людей вера в Бога дает ощущение смысла, поддержки, надежды и моральных принципов.\n" +
                    "Верить в Бога может доставлять утешение в трудные моменты, способствовать духовному развитию и поощрять доброту, эмпатию и сострадание к другим.\n\n" +
                    "Кроме того, погружение в ценности веры может привести к открытию более глубокого смысла в повседневной жизни. Вы сможете получить понимание вопросов о сущности человека, цели существования и морали.\n" +
                    "Именно вера в Бога может помочь вам обрести внутреннюю гармонию и баланс, которые помогут вам справляться с трудностями, с которыми вы сталкиваетесь на своем пути.\n\n" +
                    "Однако помните, что вера в Бога не исключает поиска знаний и развития своих навыков.\n" +
                    "Напротив, вы можете получать новые опыты, углубляться в научные открытия и развивать свои интеллектуальные способности, одновременно черпая из глубины своей веры.\n" +
                    "Именно сочетание духовности и интеллекта позволит вам более полно понять мир и приносить добавленную ценность другим людям вокруг вас.\n\n" +
                    "Таким образом, если вы действительно хотите запустить программу Stella Mod в России, помните о важных аспектах своего существования.\n" +
                    "Обдумайте свои выборы, развивайтесь интеллектуально и духовно, расширяйте горизонты через изучение языков и открытость к другим людям.\n" +
                    "Только тогда вы сможете продолжать свой путь с гордостью, осознавая смысл и ценность, которые несет ваше существование.\n\n" +
                    "После обдумывания, присоединитесь к серверу Discord и отправьте сообщение создателю. Представьте свою ценность на английском языке и объясните, почему вы хотели бы получить доступ к Stella Mod.");
                Process.Start("notepad.exe", filePath);

                Environment.Exit(2137);
            };
            closeAppTimer.Start();

            try
            {
                new ToastContentBuilder()
                    .AddText("slava poland kurwo")
                    .AddText("axaxaxaxaxaxaxa language for monkeys lol")
                    .Show();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex.ToString());
            }

            Log.Output(string.Format(Resources.Main_LoadedForm_, Text));
        }

        private void WrongCountry_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output(string.Format(Resources.Main_ClosedForm_, Text));
        }

        private void ChangeWindowSize(object sender, EventArgs e)
        {
            Width = _random.Next(200, 800);
            Height = _random.Next(200, 800);

            if (WindowState == FormWindowState.Maximized) return; // TODO

            Left = _random.Next(Screen.PrimaryScreen.Bounds.Width - Width);
            Top = _random.Next(Screen.PrimaryScreen.Bounds.Height - Height);
        }
    }
}
