using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace KioskAppNetWrapper
{
    static class Program
    {
        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        static DateTime lastRestartDate = DateTime.Parse("1970-01-01");
        static bool allowRestart = false;
        static int restartAtHour = 4;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Add handler to handle the exception raised by main threads
            Application.ThreadException +=
                new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                MailHelper.sendMail("Application Starting", "application started as planned");

                string lastRestartDateStr = SettingsHelper.ReadSetting("lastRestartDate");
                string restartAtHourStr = SettingsHelper.ReadSetting("restartAtHour");

                lastRestartDate = DateTime.Parse(lastRestartDateStr);
                if (int.TryParse(restartAtHourStr, out restartAtHour))
                {
                    restartAtHour = 4;
                }

                Application.Run(new mainWindow());
                throw new System.ArgumentException("Application Intentional Crash");
            }
            catch (Exception ex) {
                ShowExceptionDetails(ex);
            }
            

        }

        // This is the method to run when the timer is raised.
        private static void TimerEventProcessor(Object myObject,
                                                EventArgs myEventArgs)
        {
            myTimer.Stop();

            // Displays a message box asking whether to continue running the timer.
            DateTime currentDay = DateTime.Now;
            if (is5AmNextDay(currentDay))
            {
                lastRestartDate = currentDay;
                SettingsHelper.AddUpdateAppSettings("lastRestartDate", currentDay.ToLongTimeString());

                // Restarts the timer and increments the counter.
                MailHelper.sendMail("Application Restarting", "application restarting at 5:00 am as planned");
                restartHost();
            }
            else
            {
                myTimer.Enabled = true;
            }
        }

        private static void restartHost()
        {
            //restart
            allowRestart = true;
            System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
        }

        static void set5minTimer() {
            /* Adds the event and the event handler for the method that will 
          process the timer event to the timer. */
            myTimer.Tick += new EventHandler(TimerEventProcessor);

            // Sets the timer interval to 5 minutes.
            myTimer.Interval = 1000 * 60 * 5;
            myTimer.Start();
        }
        static bool is5AmNextDay(DateTime currentDay)
        {
            if (lastRestartDate.AddDays(1.0) < currentDay && currentDay.Hour == restartAtHour) {
                return true;
            }

            return false;
        }

        static void Application_ThreadException
        (object sender, System.Threading.ThreadExceptionEventArgs e)
        {// All exceptions thrown by the main thread are handled over this method

            ShowExceptionDetails(e.Exception);
        }

        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // do your logging here, write to a file, or send an email
            ShowExceptionDetails(e.ExceptionObject as Exception);

            // Suspend the current thread for now to stop the exception from throwing.
#pragma warning disable CS0618 // Type or member is obsolete
            Thread.CurrentThread.Suspend();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        static void ShowExceptionDetails(Exception Ex)
        {
            // Do logging of exception details
            ReopenApp();

        }

        static void ReopenApp()
        {
            if (!allowRestart)
            {
                MailHelper.sendMail("Application Relaunching", "some failure cause the application to relaunch");
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = "/C ping 127.0.0.1 -n 2 && \"" + Application.ExecutablePath + "\"";
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "cmd.exe";
                Process.Start(Info);
            }
            else {
                MailHelper.sendMail("Application Restarting", "application restarting at 5:00 am as planned");
            }

            Application.Exit();

        }
    }
}
