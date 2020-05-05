// based on http://alejandroruizvarela.blogspot.com/2014/01/bluetooth-arduino-xamarinandroid.html
// for this article https://habr.com/ru/post/500454/


using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Linq;
using System;
using System.IO;
using Java.Util;
using Android.Bluetooth;
using System.Threading.Tasks;

namespace _6.Canny_Xanarin_Bluetooth_Android
{
    [Activity(Label = "Control Canny 3 tiny via bluetooth", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        Button startSiren;
        TextView rSwitch;
        TextView EndSensor;
        Switch bltSwitch;
        TextView status;
        private Java.Lang.String dataToSend;
        private BluetoothAdapter mBluetoothAdapter = null;
        private BluetoothSocket btSocket = null;
        private Stream outStream = null;
        // don't forget change addres to your device:
        private static string address = "98:D3:91:F9:6C:F6";
        // MY_UUID can be saved as is
        private static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        private Stream inStream = null;


        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            startSiren = FindViewById<Button>(Resource.Id.startSiren);
            rSwitch = FindViewById<TextView>(Resource.Id.rSwitch);
            EndSensor = FindViewById<TextView>(Resource.Id.EndSensor);
            status = FindViewById<TextView>(Resource.Id.status);
            bltSwitch = FindViewById<Switch>(Resource.Id.bltSwitch);


            startSiren.Click += startSiren_ClickOnButtonClicked;
            bltSwitch.CheckedChange += bltSwitch_HandleCheckedChange;
            CheckBt();
        }

        private void CheckBt()
        {
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (!mBluetoothAdapter.Enable())
            {
                Toast.MakeText(this, "Bluetooth Desactivado",
                    ToastLength.Short).Show();
            }

            if (mBluetoothAdapter == null)
            {
                Toast.MakeText(this,
                    "Bluetooth No Existe o esta Ocupado", ToastLength.Short)
                    .Show();
            }
        }

       

        void startSiren_ClickOnButtonClicked(object sender, EventArgs e)
        {
            if (bltSwitch.Checked)
            {

                System.Console.WriteLine("gclickedddd");
                try
                {
                    dataToSend = new Java.Lang.String("11");
                    writeData(dataToSend);
                    System.Console.WriteLine("Send signal to siren");
                }
                catch (System.Exception execept)
                {
                    System.Console.WriteLine("Error when send data" + execept.Message);
                }

            }
            else status.Text = "bluetooth not connected";
        }
        void bltSwitch_HandleCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                Connect();
            }
            else
            {
                if (btSocket.IsConnected)
                {
                    try
                    {
                        btSocket.Close();
                        System.Console.WriteLine("Connection closed");
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public void Connect()
        {
            BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(address);
            System.Console.WriteLine("Connection in progress" + device);
            mBluetoothAdapter.CancelDiscovery();
            try
            {
                btSocket = device.CreateRfcommSocketToServiceRecord(MY_UUID);
                btSocket.Connect();
                System.Console.WriteLine("Correct Connection");
                status.Text = "Correct Connection to bluetooth";
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                try
                {
                    btSocket.Close();
                    System.Console.WriteLine("Connection closed");
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("Impossible to connect");
                    status.Text = "Impossible to connect";
                }
                System.Console.WriteLine("Socket Created");
  
            }
            beginListenForData();

        }

        public void beginListenForData()
        {
            try
            {
                inStream = btSocket.InputStream;
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Task.Factory.StartNew(() => {
                byte[] buffer = new byte[1024];
                int bytes;
                
                while (true)
                {

                    try
                    {
                        bytes = inStream.Read(buffer, 0, 1024);
                        System.Console.WriteLine("bytes " + bytes.ToString());
                        if (bytes > 0)
                        {
                            
                            RunOnUiThread(() => {
                                string valor = System.Text.Encoding.ASCII.GetString(buffer);
                                // transform string for deleate all symbols except 1-4(command from canny).
                                string command = new string(valor.Where(char.IsDigit).ToArray());

                                if (command.Length > 0)
                                {
                                     status.Text="data successfully readed";
                                    System.Console.WriteLine("command  " + command);
                                    switch (Int32.Parse(command))
                                    {
                                        case 0:
                                            rSwitch.Text = "reed switch - disconnected ";
                                            EndSensor.Text = "end sensor - not pressed ";
                                            break;
                                        case 1:
                                            rSwitch.Text = "reed switch - disconnected ";
                                            EndSensor.Text = "end sensor - pressed ";
                                            break;
                                        case 2:
                                            rSwitch.Text = "reed switch - connected ";
                                            EndSensor.Text = "end sensor - not pressed ";
                                            break;
                                        case 3:
                                            rSwitch.Text = "reed switch - connected ";
                                            EndSensor.Text = "end sensor - pressed ";
                                        break;
                                    }
                                }
                            });
                        }
                    }
                    catch (Java.IO.IOException)
                    {
                        RunOnUiThread(() => {
                            EndSensor.Text = "End sensor status - undefined";
                            rSwitch.Text = "Reed switch status - undefined ";
                        });
                        break;
                    }
                }
            });
        }

        private void writeData(Java.Lang.String data)
        {
            try
            {
                outStream = btSocket.OutputStream;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error with OutputStream when write to Serial port" + e.Message);
            }

            Java.Lang.String message = data;

            byte[] msgBuffer = message.GetBytes();

            try
            {
                outStream.Write(msgBuffer, 0, msgBuffer.Length);
                System.Console.WriteLine("Message sent");
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error with  when write message to Serial port" + e.Message);
                status.Text = "Error with  when write message to Serial port";
            }
        }


    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
 }


 