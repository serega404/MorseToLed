using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using MorseSharp;
using MorseSharp.Converter;

TextMorseConverter converter = new TextMorseConverter(Language.English);
const int redPin = 17;
const int greenPin = 27;
const int bluePin = 22;

// Здесь мы используем SysFsDriver, потому что он быстрее всех остальных
// https://github.com/dotnet/iot/issues/1490
using GpioController gpio = new GpioController(PinNumberingScheme.Logical, new SysFsDriver());

gpio.OpenPin(redPin, PinMode.Output);
gpio.OpenPin(greenPin, PinMode.Output);
gpio.OpenPin(bluePin, PinMode.Output);
gpio.Write(redPin, PinValue.High);
gpio.Write(greenPin, PinValue.High);
gpio.Write(bluePin, PinValue.High);

var morse = await converter.ConvertTextToMorse("Hello world");
Console.WriteLine("Morse: " + morse);
while (true)
{
    Console.Write("Enter speed multiplier: ");
    int speedMultiplier = Convert.ToInt32(Console.ReadLine());
    Console.Write("Morse: ");
    for (int i = 0; i < morse.Length; i++)
    {
        Console.Write(morse[i]);
        switch (morse[i])
        {
            case '.':
                gpio.Write(redPin, PinValue.Low);
                Thread.Sleep(1 * speedMultiplier);
                if (morse[i + 1] is not ' ' and not '/')
                {
                    gpio.Write(redPin, PinValue.High);
                    Thread.Sleep(1 * speedMultiplier);
                }
                break;
            case '_':
                gpio.Write(redPin, PinValue.Low);
                Thread.Sleep(3 * speedMultiplier);
                if (morse[i + 1] is not ' ' and not '/')
                {
                    gpio.Write(redPin, PinValue.High);
                    Thread.Sleep(1 * speedMultiplier);
                }
                break;
            case '/':
                gpio.Write(redPin, PinValue.High);
                Thread.Sleep(1 * speedMultiplier);
                break;
            case ' ':
                gpio.Write(redPin, PinValue.High);
                Thread.Sleep(3 * speedMultiplier);
                break;
            default:
                Console.WriteLine("Unknown morse code: " + morse[i]);
                break;
        }
    }
    Console.WriteLine();
}

gpio.ClosePin(redPin);
gpio.ClosePin(greenPin);
gpio.ClosePin(bluePin);