using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO.Ports;

namespace GUI
{
    public class SerialPortManager
    {
        public SerialPort SerialPort { get; private set; }

        public SerialPortManager(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake)
        {
            SerialPort = new SerialPort
            {
                PortName = portName,
                BaudRate = baudRate,
                Parity = parity,
                DataBits = dataBits,
                StopBits = stopBits,
                Handshake = handshake
            };

            SerialPort.DataReceived += SerialPort_DataReceived;
        }

        public void OpenPort()
        {
            if (!SerialPort.IsOpen)
            {
                SerialPort.Open();
            }
        }

        public void ClosePort()
        {
            if (SerialPort.IsOpen)
            {
                SerialPort.Close();
            }
        }

        public void SendData(string data)
        {
            if (SerialPort.IsOpen)
            {
                SerialPort.WriteLine(data);
            }
        }

        public event EventHandler<string> DataReceived;

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = SerialPort.ReadLine();
                DataReceived?.Invoke(this, data);
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }
    }
}