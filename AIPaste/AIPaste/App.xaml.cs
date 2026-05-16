using System;
using System.Threading.Tasks;
using AIPaste.common;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AIPaste
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            CrashDiagnostics.Initialize();
            RegisterGlobalExceptionHandlers();
            CrashDiagnostics.WriteTrace("App constructor started");
            this.InitializeComponent();
            var logger = MyLogger.GetInstance();
            #if DEBUG
                logger.SetDevelopmentMode(true);
            #endif
            logger.Info("START_APP");
            CrashDiagnostics.WriteTrace("App constructor completed");
        }

        public static MainWindow? MainWindow;

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            CrashDiagnostics.WriteTrace("OnLaunched entered");
            try
            {
                MainWindow = new MainWindow();
                CrashDiagnostics.WriteTrace("MainWindow created");
                MainWindow.Activate();
                CrashDiagnostics.WriteTrace("MainWindow activated");
            }
            catch (Exception ex)
            {
                CrashDiagnostics.WriteException("App.OnLaunched", ex);
                throw;
            }
        }

        private void RegisterGlobalExceptionHandlers()
        {
            UnhandledException += OnUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            CrashDiagnostics.WriteException("Application.UnhandledException", e.Exception);
        }

        private void OnCurrentDomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                CrashDiagnostics.WriteException("AppDomain.CurrentDomain.UnhandledException", exception);
                return;
            }

            CrashDiagnostics.WriteTrace($"AppDomain.CurrentDomain.UnhandledException received non-Exception payload: {e.ExceptionObject}");
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            CrashDiagnostics.WriteException("TaskScheduler.UnobservedTaskException", e.Exception);
        }
    }
}
