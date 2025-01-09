// See https://aka.ms/new-console-template for more information

var nameIndex = args
    .Select((value, idx) => new { Value = value, Index = idx })
    .FirstOrDefault(x => x.Value.Equals("-n"))?.Index ?? -1;
if (args.Length == 0)
{
    args = ["World"];
}

Console.WriteLine($"Hello, {args[nameIndex + 1]}!");