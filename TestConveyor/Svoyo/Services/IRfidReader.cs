namespace TestConveyor.Svoyo.Services
{
    interface IRfidReader
    {
        bool Connect(string address);

        void Start();

        void Stop();
    } 
}
