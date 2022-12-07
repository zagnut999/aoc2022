using System.Collections.ObjectModel;

namespace aoc2022.Day07;

[TestFixture]
internal class Tests
{
    public abstract class Node
    {
        public string Name { get; }

        protected Node(string name)
        {
            Name = name;
        }
    }

    public class File : Node
    {
        public long Size { get; }
        
        public File(string name, long size) : base(name)
        {
            Size = size;
        }

        public override string ToString()
        {
            return $"File {Name}, size:{Size}";
        }
    }

    public class Directory : Node
    {
        public Directory? Parent { get; }

        private readonly List<Node> _children = new List<Node>();

        public ReadOnlyCollection<Node> Children => _children.AsReadOnly();

        private long _size;
        private bool _isDirty;

        public long Size
        {
            get
            {
                if (_isDirty)
                {
                    _isDirty = false;
                    _size = CalcSize();
                }

                return _size;
            }
        }

        public static Directory CreateRoot()
        {
            return new Directory("/");
        }

        private Directory(string name) : base(name)
        {
            Parent = null;
            _isDirty = true;
        }

        public Directory(string name, Directory parent) : this(name)
        {
            Parent = parent;
            var node = parent;
            while (node != null)
            {
                node._isDirty = true;
                node = node.Parent;
            }
        }

        public void AddChild(Node node)
        {
            _isDirty = true;
            var node2 = Parent;
            while (node2 != null)
            {
                node2._isDirty = true;
                node2 = node2.Parent;
            }

            _children.Add(node);
        }

        private long CalcSize()
        {
            var fileSize = Children.OfType<File>().Sum(x=> x.Size);
            var directories = Children.OfType<Directory>();

            foreach (var directory in directories)
            {
                fileSize += directory.Size;
            }

            return fileSize;
        }

        public override string ToString()
        {
            return $"Directory {Name}, size:{Size}";
        }
    }

    public class State
    {
        private Directory _current;
        private readonly List<Directory> _directories = new();

        public ReadOnlyCollection<Directory> Directories => _directories.AsReadOnly();

        public Directory Root { get; }
        
        public State()
        {
            Root = Directory.CreateRoot();
            _current = Root;
        }

        public State CD(string name)
        {
            switch (name)
            {
                case "/":
                    _current = Root;
                    break;
                case "..":
                    _current = _current.Parent ?? Root;
                    break;
                default:
                {
                    var directory =  (Directory?) _current.Children.FirstOrDefault(x=>x is Directory && x.Name == name);

                    _current = directory ?? throw new ArgumentException($"Directory Not Found: {name}");
                    break;
                }
            }

            return this;
        }

        public long GetTotalSize()
        {
            return _current.Size;
        }

        public ReadOnlyCollection<Directory> DirectorySizeAtMost(long size)
        {
            return _directories.Where(x=>x.Size < size).ToList().AsReadOnly();
        }
        
        public void ProcessInput(List<string> input)
        {
            foreach (var line in input)
            {
                if (line.StartsWith("$"))
                {
                    var split = line.Split(" ");

                    switch (split[1])
                    {
                        case "cd":
                            CD(split[2]);
                            break;
                        case "ls":
                            //process folder list
                            break;
                    }
                }
                else
                {
                    ProcessDirectoryEntry(line);
                }
            }
        }

        private void ProcessDirectoryEntry(string raw)
        {
            var split = raw.Split(' ');
            if (split[0] == "dir")
            {
                var directory = new Directory(split[1], _current);
                _current.AddChild(directory);
                _directories.Add(directory);
            }
            else
                _current.AddChild(new File(split[1], long.Parse(split[0])));
        }
    }

    private readonly List<string> _sample = new List<string>()
    {
        "$ cd /",
        "$ ls",
        "dir a",
        "14848514 b.txt",
        "8504156 c.dat",
        "dir d",
        "$ cd a",
        "$ ls",
        "dir e",
        "29116 f",
        "2557 g",
        "62596 h.lst",
        "$ cd e",
        "$ ls",
        "584 i",
        "$ cd ..",
        "$ cd ..",
        "$ cd d",
        "$ ls",
        "4060174 j",
        "8033020 d.log",
        "5626152 d.ext",
        "7214296 k"
    };

    [Test]
    public void ProcessInput()
    {
        var state = new State();
        state.ProcessInput(_sample);

        state.CD("/").CD("a").CD("e");
        state.GetTotalSize().ShouldBe(584);

        state.CD("/").CD("a");
        state.GetTotalSize().ShouldBe(94853);

        state.CD("/").CD("d");
        state.GetTotalSize().ShouldBe(24933642);

        state.CD("/");
        state.GetTotalSize().ShouldBe(48381165);
    }

    [Test]
    public void Example()
    {
        var state = new State();
        state.ProcessInput(_sample);
        state.DirectorySizeAtMost(100000).Sum(x =>x.Size).ShouldBe(95437);
    }

    [Test]
    public async Task ReadDaysFile()
    {
        var list = await Utilities.ReadInputByDay("Day07");
        list.ShouldNotBeEmpty();
    }

    [Test]
    public async Task Actual()
    {
        var list = await Utilities.ReadInputByDay("Day07");
        var state = new State();
        state.ProcessInput(list);
        state.DirectorySizeAtMost(100000).Sum(x => x.Size).ShouldBe(1491614);
    }

    [Test]
    public void ExamplePart2()
    {
        var totalSpace = 70000000L;
        var spaceNeeded = 30000000L;

        var state = new State();
        state.ProcessInput(_sample);

        state.CD("/");
        var usedSpace = state.CD("/").GetTotalSize();
        var freeSpace = totalSpace - usedSpace;
        var spaceToFree = spaceNeeded - freeSpace;

        var options = state.Directories.Where(x => x.Size >= spaceToFree).OrderBy(x => x.Size);
        options.First().Name.ShouldBe("d");
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await Utilities.ReadInputByDay("Day07");
        var totalSpace = 70000000L;
        var spaceNeeded = 30000000L;

        var state = new State();
        state.ProcessInput(list);

        state.CD("/");
        var usedSpace = state.CD("/").GetTotalSize();
        var freeSpace = totalSpace - usedSpace;
        var spaceToFree = spaceNeeded - freeSpace;

        var options = state.Directories.Where(x => x.Size >= spaceToFree).OrderBy(x => x.Size).ToList();
        options.First().Size.ShouldBe(6400111L);
    }
}