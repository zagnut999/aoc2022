using System.Text;

namespace aoc2022.Day10;

[TestFixture]
internal class Tests
{
    private class Cpu
    {
        public int X { get; private set; }

        private readonly List<Command> _commands = new();
        private readonly Dictionary<string, int> _cycles = new()
        {
            {"noop", 1},
            {"addx", 2}
        };

        public Cpu(Clock clock)
        {
            X = 1;

            clock.OnTickStarted += TickStartedHandler;
            clock.OnTickEnding += TickEndingHandler;
        }

        public readonly List<int> SignalStrengths = new();
        private int _nextTickToRecord = 20;

        private void TickStartedHandler(object? sender, TickEventArgs args)
        {
            if (args.Tick == _nextTickToRecord)
            {
                _nextTickToRecord += 40;
                var strength = args.Tick * X;
                SignalStrengths.Add(strength);
            }
        }

        private void TickEndingHandler(object? sender, TickEventArgs args)
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

                _commands.Add(new(command, value, cycles));
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
        var clock = new Clock();
        var cpu = new Cpu(clock);

        cpu.AddInstructions(new() { "noop", "addx 3", "addx -5" });

        clock.Tick();
        cpu.X.ShouldBe(1);

        clock.Tick();
        cpu.X.ShouldBe(1);

        clock.Tick();
        cpu.X.ShouldBe(4);

        clock.Tick();
        cpu.X.ShouldBe(4);

        clock.Tick();
        cpu.X.ShouldBe(-1);
    }


    [Test]
    public void Example()
    {

        var clock = new Clock();
        var cpu = new Cpu(clock);

        cpu.AddInstructions(_sample);

        for (var tick = 1; tick < 241; tick++)
        {
            clock.Tick();
        }

        var signalStrengths = cpu.SignalStrengths;
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

        var clock = new Clock();
        var cpu = new Cpu(clock);
        cpu.AddInstructions(list);

        for (var tick = 1; tick < 241; tick++)
        {
            clock.Tick();
        }

        var signalStrengths = cpu.SignalStrengths;
        signalStrengths.Take(6).Sum(x => x).ShouldBe(12980);
    }

    private class Crt
    {
        private readonly Cpu _cpu;

        private static readonly int Width = 40;
        private static readonly int Height = 6;

        private readonly List<char[]> _screen = new();

        private int _scanIndex;
        private (int x, int y) GetScanCoordinates() => (_scanIndex % Width, _scanIndex / Width);

        public Crt(Clock clock, Cpu cpu)
        {
            _cpu = cpu;

            for (var row = 0; row < Height; row++)
            {
                _screen.Add(new string(' ', Width).ToCharArray());
            }

            clock.OnTickStarted += TickStartedHandler;
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

        private void TickStartedHandler(object? sender, TickEventArgs args)
        {
            var registerX = _cpu.X;
            var (x, y) = GetScanCoordinates();

            if (registerX - 1 <= x && x <= registerX + 1)
            {
                _screen[y][x] = '#';
            }

            _scanIndex++;
        }
    }

    private class TickEventArgs : EventArgs
    {
        public TickEventArgs(int tick)
        {
            Tick = tick;
        }

        public int Tick { get; }
    }

    private class Clock
    {
        public event EventHandler<TickEventArgs>? OnTickStarted;
        public event EventHandler<TickEventArgs>? OnTickEnding;

        private int _currentTick;

        public void Tick()
        {
            _currentTick++;
            var args = new TickEventArgs(_currentTick);

            OnTickStart(args);

            OnTickEnd(args);
        }

        private void OnTickStart(TickEventArgs args)
        {
            OnTickStarted?.Invoke(this, args);
        }

        private void OnTickEnd(TickEventArgs args)
        {
            OnTickEnding?.Invoke(this, args);
        }
    }

    [Test]
    public void ExamplePart2()
    {
        var clock = new Clock();
        var cpu = new Cpu(clock);
        cpu.AddInstructions(_sample);

        var crt = new Crt(clock, cpu);

        for (var tick = 0; tick < 240; tick++)
        {
            clock.Tick();
        }

        Console.WriteLine(crt);
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await Utilities.ReadInputByDay("Day10");
        var clock = new Clock();
        var cpu = new Cpu(clock);
        cpu.AddInstructions(list);

        var crt = new Crt(clock, cpu);

        for (var tick = 0; tick < 240; tick++)
        {
            clock.Tick();
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