using System.Text;

namespace aoc2022.Day10;

[TestFixture]
internal class Tests
{
    public interface ITick
    {
        void DuringTick();
        void AtEndOfTick();
    }

    private class Cpu : ITick
    {
        public int X { get; private set; }

        private readonly List<Command> _commands = new();
        private readonly Dictionary<string, int> _cycles = new()
        {
            {"noop", 1},
            {"addx", 2}
        };

        public Cpu()
        {
            X = 1;
        }

        public void DuringTick()
        {
            // Look busy
        }

        public void AtEndOfTick()
        {
            if (_commands.Any(x => x.CyclesLeft != 0))
            {
                var command = _commands.First(x => x.CyclesLeft != 0);

                command.CyclesLeft--;

                if (command.CyclesLeft == 0)
                    ProcessInstruction(command.Op, command.Value);
            }
        }

        public void AddInstructions(List<string> instructions)
        {
            foreach (var instruction in instructions)
            {
                var (command, value) = ParseInstruction(instruction);
                var cycles = _cycles[command];

                _commands.Add(new (command, value, cycles));
            }
        }
        
        private void ProcessInstruction(string op, int? value)
        {
            switch (op)
            {
                case "noop": break;
                case "addx": 
                    X += value ?? 0;
                    break;
            }
        }

        private (string command, int? value) ParseInstruction(string instruction)
        {
            var parts = instruction.Split(" ");
            var command = parts[0];
            int? value = null;

            if (parts.Length == 2 && int.TryParse(parts[1], out var parsedValue))
                value = parsedValue;
            
            return (command, value);
        }

        private class Command
        {
            public string Op { get; }
            public int? Value { get; }
            public int CyclesLeft { get; set; }

            public Command(string op, int? value, int cyclesLeft)
            {
                Op = op;
                Value = value;
                CyclesLeft = cyclesLeft;
            }
        }
    }

    [Test]
    public void CpuTests()
    {
        var cpu = new Cpu();
        cpu.AddInstructions(new () { "noop", "addx 3", "addx -5"});

        cpu.AtEndOfTick();
        cpu.X.ShouldBe(1);

        cpu.AtEndOfTick();
        cpu.X.ShouldBe(1);

        cpu.AtEndOfTick();
        cpu.X.ShouldBe(4);

        cpu.AtEndOfTick();
        cpu.X.ShouldBe(4);

        cpu.AtEndOfTick();
        cpu.X.ShouldBe(-1);
    }


    [Test]
    public void Example()
    {
        var cpu = new Cpu();
        var clock = new Clock();
        clock.RegisterTicker(cpu);

        cpu.AddInstructions(_sample);
        var signalStrengths = new List<int>();
        var nextTickToRecord = 20;

        for (var tick = 1; tick < 241; tick++)
        {
            clock.StartTick();

            //During this tick
            if (tick == nextTickToRecord)
            {
                nextTickToRecord += 40;
                var strength = tick * cpu.X;
                signalStrengths.Add(strength);
            }

            //after tick ends
            clock.EndTick();
        }

        signalStrengths[0].ShouldBe(420);
        signalStrengths[1].ShouldBe(1140);
        signalStrengths[2].ShouldBe(1800);
        signalStrengths[3].ShouldBe(2940);
        signalStrengths[4].ShouldBe(2880);
        signalStrengths[5].ShouldBe(3960);

        signalStrengths.Take(6).Sum(x => x).ShouldBe(13140);
    }

    [Test]
    public async Task ReadDaysFile()
    {
        var list = await Utilities.ReadInputByDay("Day10");
        list.ShouldNotBeEmpty();
    }

    [Test]
    public async Task Actual()
    {
        var list = await Utilities.ReadInputByDay("Day10");
        var cpu = new Cpu();
        cpu.AddInstructions(list);

        var clock = new Clock();
        clock.RegisterTicker(cpu);

        var signalStrengths = new List<int>();
        var nextTickToRecord = 20;

        for (var tick = 1; tick < 241; tick++)
        {
            clock.StartTick();

            if (tick == nextTickToRecord)
            {
                nextTickToRecord += 40;
                var strength = tick * cpu.X;
                signalStrengths.Add(strength);
            }

            clock.EndTick();
        }

        signalStrengths.Take(6).Sum(x => x).ShouldBe(12980);
    }

    private class Crt : ITick
    {
        private readonly Cpu _cpu;

        private static readonly int Width = 40;
        private static readonly int Height = 6;

        private readonly List<char[]> _screen = new();
        
        private int _scanIndex;
        private (int x, int y) GetScanCoordinates() => (_scanIndex % Width, _scanIndex / Width);

        public Crt(Cpu cpu)
        {
            _cpu = cpu;

            for (var row = 0; row < Height; row++)
            {
                _screen.Add(new string(' ', Width).ToCharArray());
            }
        }

        public void DuringTick()
        {
            var registerX = _cpu.X;
            var (x, y) = GetScanCoordinates();

            if (registerX - 1 <= x && x <= registerX + 1)
            {
                _screen[y][x] = '#';
            }

            _scanIndex++;
        }

        public void AtEndOfTick()
        {
            // Put a bow on it
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var line in _screen)
            {
                result.Append("[").Append(line).Append("]").AppendLine();
            }
            
            return result.ToString();
        }
    }

    private class Clock
    {
        private readonly List<ITick> _tickers = new();

        public void RegisterTicker(ITick ticker)
        {
            _tickers.Add(ticker);
        }

        public void StartTick()
        {
            foreach (var ticker in _tickers)
            {
                ticker.DuringTick();
            }
        }

        public void EndTick()
        {
            foreach (var ticker in _tickers)
            {
                ticker.AtEndOfTick();
            }
        }
    }

    [Test]
    public void ExamplePart2()
    {
        var cpu = new Cpu();
        cpu.AddInstructions(_sample);

        var crt = new Crt(cpu);

        var clock = new Clock();
        clock.RegisterTicker(cpu);
        clock.RegisterTicker(crt);

        for (var tick = 0; tick < 240; tick++)
        {
            clock.StartTick();
            
            clock.EndTick();
        }

        Console.WriteLine(crt);
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await Utilities.ReadInputByDay("Day10");

        var cpu = new Cpu();
        cpu.AddInstructions(list);

        var crt = new Crt(cpu);

        var clock = new Clock();
        clock.RegisterTicker(cpu);
        clock.RegisterTicker(crt);

        for (var tick = 0; tick < 240; tick++)
        {
            clock.StartTick();

            clock.EndTick();
        }

        Console.WriteLine(crt);
    }

    private readonly List<string> _sample = new()
    {
        "addx 15",
        "addx -11",
        "addx 6",
        "addx -3",
        "addx 5",
        "addx -1",
        "addx -8",
        "addx 13",
        "addx 4",
        "noop",
        "addx -1",
        "addx 5",
        "addx -1",
        "addx 5",
        "addx -1",
        "addx 5",
        "addx -1",
        "addx 5",
        "addx -1",
        "addx -35",
        "addx 1",
        "addx 24",
        "addx -19",
        "addx 1",
        "addx 16",
        "addx -11",
        "noop",
        "noop",
        "addx 21",
        "addx -15",
        "noop",
        "noop",
        "addx -3",
        "addx 9",
        "addx 1",
        "addx -3",
        "addx 8",
        "addx 1",
        "addx 5",
        "noop",
        "noop",
        "noop",
        "noop",
        "noop",
        "addx -36",
        "noop",
        "addx 1",
        "addx 7",
        "noop",
        "noop",
        "noop",
        "addx 2",
        "addx 6",
        "noop",
        "noop",
        "noop",
        "noop",
        "noop",
        "addx 1",
        "noop",
        "noop",
        "addx 7",
        "addx 1",
        "noop",
        "addx -13",
        "addx 13",
        "addx 7",
        "noop",
        "addx 1",
        "addx -33",
        "noop",
        "noop",
        "noop",
        "addx 2",
        "noop",
        "noop",
        "noop",
        "addx 8",
        "noop",
        "addx -1",
        "addx 2",
        "addx 1",
        "noop",
        "addx 17",
        "addx -9",
        "addx 1",
        "addx 1",
        "addx -3",
        "addx 11",
        "noop",
        "noop",
        "addx 1",
        "noop",
        "addx 1",
        "noop",
        "noop",
        "addx -13",
        "addx -19",
        "addx 1",
        "addx 3",
        "addx 26",
        "addx -30",
        "addx 12",
        "addx -1",
        "addx 3",
        "addx 1",
        "noop",
        "noop",
        "noop",
        "addx -9",
        "addx 18",
        "addx 1",
        "addx 2",
        "noop",
        "noop",
        "addx 9",
        "noop",
        "noop",
        "noop",
        "addx -1",
        "addx 2",
        "addx -37",
        "addx 1",
        "addx 3",
        "noop",
        "addx 15",
        "addx -21",
        "addx 22",
        "addx -6",
        "addx 1",
        "noop",
        "addx 2",
        "addx 1",
        "noop",
        "addx -10",
        "noop",
        "noop",
        "addx 20",
        "addx 1",
        "addx 2",
        "addx 2",
        "addx -6",
        "addx -11",
        "noop",
        "noop",
        "noop",
    };
}