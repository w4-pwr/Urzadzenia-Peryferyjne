using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PCSC;

namespace ConsoleApplication1
{
    class Program
    {


        private static SCardError err;
        private static SCardReader reader;
        private static System.IntPtr protocol;
        private static SCardContext hContext;
        static void Main(string[] args)
        {
            try
            {
                connect();

                byte[] commandBytes = new byte[] { 0xA0, 0xA4, 0x00, 0x00, 0x02, 0x7F, 0x10 };
                sendCommand(commandBytes, "SELECT TELECOM");

                // SELECT TELECOM
                commandBytes = new byte[] {0xA0, 0xA4, 0x00, 0x00, 0x02, 0x7F, 0x10};
                sendCommand(commandBytes, "SELECT TELECOM");

                // GET RESPONSE
                commandBytes = new byte[] {0xA0, 0xC0, 0x00, 0x00, 0x16};
                sendCommand(commandBytes, "GET RESPONSE");

                // SELECT SMS
                commandBytes = new byte[] {0xA0, 0xA4, 0x00, 0x00, 0x02, 0x6F, 0x3C};
                sendCommand(commandBytes, "SELECT SMS");

                // GET RESPONSE
                commandBytes = new byte[] {0xA0, 0xC0, 0x00, 0x00, 0x0F};
                sendCommand(commandBytes, "GET RESPONSE");

                // READ RECORD //3 numer rekordu, 4 mode
                commandBytes = new byte[] {0xA0, 0xB2, 0x01, 0x04, 0xB0};
                sendCommand(commandBytes, "READ RECORD");

                hContext.Release();
            }
            catch (PCSCException ex)
            {
                Console.WriteLine("Blad: " + ex.Message + " (" + ex.SCardError.ToString() + ")");
            }
            catch (Exception e)
            {
                Console.WriteLine("nieznany blad");
            }
            Console.ReadLine();
        }

        public static void toTelecom()
        {
             byte[] commandBytes = new byte[] {0xA0, 0xA4, 0x00, 0x00, 0x02, 0x7F, 0x10};
                sendCommand(commandBytes, "SELECT TELECOM");

                // GET RESPONSE
                commandBytes = new byte[] {0xA0, 0xC0, 0x00, 0x00, 0x16};
                sendCommand(commandBytes, "GET RESPONSE");

                // SELECT SMS
                commandBytes = new byte[] {0xA0, 0xA4, 0x00, 0x00, 0x02, 0x6F, 0x3C};
                sendCommand(commandBytes, "SELECT SMS");

                // GET RESPONSE
                commandBytes = new byte[] {0xA0, 0xC0, 0x00, 0x00, 0x0F};
                sendCommand(commandBytes, "GET RESPONSE");

        }

        public static void connect()
        {
            hContext = new SCardContext();
            hContext.Establish(SCardScope.System);

            string[] readerList = hContext.GetReaders();
            Boolean noReaders = readerList.Length <= 0;
            if (noReaders)
            {
                throw new PCSCException(SCardError.NoReadersAvailable, "Blad czytnika");
            }

            Console.WriteLine("Nazwa czytnika: " + readerList[0]);

            reader = new SCardReader(hContext);

            err = reader.Connect(readerList[0],
                SCardShareMode.Shared,
                SCardProtocol.T0 | SCardProtocol.T1);
            checkError(err);

            switch (reader.ActiveProtocol)
            {
                case SCardProtocol.T0:
                    protocol = SCardPCI.T0;
                    break;
                case SCardProtocol.T1:
                    protocol = SCardPCI.T1;
                    break;
                default:
                    throw new PCSCException(SCardError.ProtocolMismatch, "nieobslugiwany protokol: "+ reader.ActiveProtocol.ToString());
            }
        }

        public static void sendCommand(byte[] comand, String name)
        {
            byte[] recivedBytes = new byte[256];
            err = reader.Transmit(protocol, comand, ref recivedBytes);
            checkError(err);
            writeResponse(recivedBytes, name);
        }

        public static void writeResponse(byte[] recivedBytes, String responseCode)
        {
                            Console.Write(responseCode+ ": ");
                            for (int i = 0; i < recivedBytes.Length; i++)
                                Console.Write("{0:X2} ", recivedBytes[i]);
            Console.WriteLine();
        }

        static void checkError(SCardError err)
        {
            if (err != SCardError.Success)
            {
                throw new PCSCException(err, SCardHelper.StringifyError(err));
            }
        }

    }
}
