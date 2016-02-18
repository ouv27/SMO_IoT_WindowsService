using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration.Install;
using uPLibrary.Networking.M2Mqtt;
using SMO_IoT;

namespace SMO_IoT_WindowsService
{
    [RunInstaller(true)]
    public class ServiceInstall : Installer
    {
        public ServiceInstall() : base()
        {
            // On définit le compte sous lequel le service sera lancé (compte Système)
            ServiceProcessInstaller process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;

            // On définit le mode de lancement (Manuel), le nom du service et sa description
            ServiceInstaller service = new ServiceInstaller();
            service.StartType = ServiceStartMode.Automatic;
            service.ServiceName = "_SMO_Service";
            service.DisplayName = "_SMO_Service";
            service.Description = "Service de test pour SMO_IoT";

            // On ajoute les installeurs à la collection (l'ordre n'a pas d'importance) 
            Installers.Add(service);
            Installers.Add(process);
        }
    }

    public partial class SMO_IoT_WindowsService : ServiceBase
    {
        private Timer t = null;
        public SMO_IoT_WindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            t = new Timer(10000); // Timer de 10 secondes.
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.Start();
            // create client instance 
            MqttClient client = new MqttClient("test.mosquitto.org");

            client.Connect("SMO_IoT_WindowsService", null, null);
            // publish a message on "/home/temperature" topic with QoS 2 
            IoT_Topics iot = new IoT_Topics();
            Iot_Constants IOTCONST = new Iot_Constants();
            string sTopic = iot.Get_Topics("", "");
            client.Publish("ou", Encoding.UTF8.GetBytes(sTopic));
        }

        protected override void OnStop()
        {
            t.Stop();
        }
        protected void t_Elapsed(object sender, EventArgs e)
        {

            if (File.Exists(@"C:\temp\test.txt"))
            {
                StreamWriter sw = new StreamWriter(@"C:\temp\test.txt");
                sw.WriteLine(DateTime.Now.ToString());
                sw.Close();
            }
            else
            {
                TextWriter file = File.CreateText(@"C:\temp\test.txt");
                file.WriteLine(DateTime.Now.ToString());
                file.Close();
            }
        }

    }



}
