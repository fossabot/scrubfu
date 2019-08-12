# C# Style Guide

The intention of this guide is to provide a set of conventions that encourage good code.
It is the distillation of many combined man-years of software engineering and C# development
experience.  While some suggestions are more strict than others, good judgment should always be practiced.

If following the guide causes unnecessary hoop-jumping or otherwise less-readable code,
*readability trumps the guide*.  However, if the more 'readable' variant comes with
perils or pitfalls, readability may be sacrificed.

In general, much of the project style and conventions mirror the
[Microsoft best practice coding conventions for C# projects](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).

## Table of contents

- [Coding style](#coding-style)
  - [Formatting](#formatting)
  - [Variable naming](#variable-naming)
  - [Implicitly Typed Local Variables](#implicitly-typed-local-variables-[1]-[2])
  - [Unsigned Data Type](#unsigned-data-type-[1])
  - [Arrays](#arrays-[1])
  - [Delegates](#delegates-[1])
  - [Static Members](#static-members-[1]-[2])
  - [LINQ Queries](#linq-queries-[1])
  - [Space pad operators and equals](#space-pad-operators-and-equals)
  - [Be explicit about operator precedence](#be-explicit-about-operator-precedence)
  - [Documentation](#documentation)
  - [Use annotations wisely](#use-annotations-wisely)
  - [Use interfaces](#use-interfaces)
- [Writing testable code](#writing-testable-code)
  - [Fakes and mocks](#fakes-and-mocks)
  - [Let your callers construct support objects](#let-your-callers-construct-support-objects)
  - [Testing multithreaded code](#testing-multithreaded-code)
  - [Testing antipatterns](#testing-antipatterns)
- [Avoid randomness in tests](#avoid-randomness-in-tests)
- [Best practices](#best-practices)
  - [Defensive programming](#defensive-programming)
  - [Clean code](#clean-code)
  - [Use newer/better libraries](#use-newerbetter-libraries)
  - [equals() and hashCode()](#equals-and-hashcode)
  - [Premature optimization is the root of all evil](#premature-optimization-is-the-root-of-all-evil)
  - [TODOs](#todos)
  - [Obey the Law of Demeter](#obey-the-law-of-demeter-lod)
  - [Don't Repeat Yourself](#dont-repeat-yourself-dry)
  - [Manage threads properly](#manage-threads-properly)
  - [Avoid unnecessary code](#avoid-unnecessary-code)
  - [The 'fast' implementation](#the-fast-implementation)

## Coding style

### Formatting

#### General [1]

- Write only one statement per line.
- Write only one declaration per line.
- Add at least one blank line between method definitions and property definitions.

#### Commenting Conventions [1]

- Place the comment on a separate line, not at the end of a line of code.
- Begin comment text with an uppercase letter.
- End comment text with a period.
- Insert one space between the comment delimiter (//) and the comment text.
- Do not create formatted blocks of asterisks around comments.

#### String Data Type [1] [2]

Use [string interpolation](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated) to concatenate short strings, as shown in the following code:

    :::C#
    string displayName = $"{nameList[n].LastName}, {nameList[n].FirstName}";

To append strings in loops, especially when you are working with large amounts of text, use a StringBuilder object.

    :::C#
    var phrase = "lalalalalalalalalalalalalalalalalalalalalalalalalalalalalala";
    var manyPhrases = new StringBuilder();

    for (var i = 0; i < 10000; i++)
    {
        manyPhrases.Append(phrase);
    }

    //Console.WriteLine("tra" + manyPhrases);

#### Use line breaks wisely

There are generally two reasons to insert a line break:

1. A statement exceeds the column limit.

2. A thought needs to be  logically separated.

Writing code is like telling a story.  Written language constructs like chapters, paragraphs, and punctuation (e.g. semicolons, commas, periods, hyphens) convey thought hierarchy and separation.  Programming languages have similar constructs, and should be used to effectively tell the story to those reading the code.

#### Indent style

The [Allman](https://en.wikipedia.org/wiki/Indentation_style#Allman_style) style is used.
Indent size is 1 tab or 4 columns.

    :::C#
    // Like this.
    if (x < 0) 
    {
        Negative(x);
    } 
    else
    {
        NonNegative(x);
    }

    // Or like this
    if (x < 0)
        Negative(x);

    // Notlike this.
    if (x < 0) Negative(x);

Continuation indent is 4 columns.  Nested continuations add another 4 columns.

    :::C#
    // Bad.
    //   - Line breaks are arbitrary.
    //   - Scanning the code makes it difficult to piece the message together.
    throw new Exception("Failed to process request" + request.Id
        + " for user " + user.Id + " query: '" + query.Text
        + "'");

    // Good.
    //   - Each component of the message is separate and self-contained.
    //   - Adding or removing a component of the message requires minimal reformatting.
    throw new Exception("Failed to process"
        + " request " + request.Id
        + " for user " + user.Id
        + " query: '" + query.Text + "'");

Don't break up a statement unnecessarily.

    :::C#
    // Bad.
    const string value =
        otherValue;

    // Good.
    const string value = otherValue;

Method declaration continuations.

    :::C#
    // Sub-optimal since line breaks are arbitrary and only filling lines.
    string DownloadAnInternet(Internet internet, Tubes tubes,
        Blogosphere blogs, Dictionary<Long, Data> bandwidth) 
    {
      tubes.download(internet);
      ...
    }

    // Acceptable.
    string DownloadAnInternet(Internet internet, Tubes tubes, Blogosphere blogs,
        Dictionary<Long, Data> bandwidth) 
    {
      tubes.download(internet);
      ...
    }

    // Nicer, as the extra newline gives visual separation to the method body.
    string DownloadAnInternet(Internet internet, Tubes tubes, Blogosphere blogs,
        Dictionary<Long, Data> bandwidth) 
    {

      tubes.download(internet);
      ...
    }

    // Also acceptable, but may be awkward depending on the column depth of the opening parenthesis.
    public string DownloadAnInternet(Internet internet,
                                     Tubes tubes,
                                     Blogosphere blogs,
                                     Dictionary<Long, Data> bandwidth) 
    {
      tubes.download(internet);
      ...
    }

    // Preferred for easy scanning and extra column space.
    public string DownloadAnInternet(
        Internet internet,
        Tubes tubes,
        Blogosphere blogs,
        Dictionary<Long, Data> bandwidth) 
    {

      tubes.download(internet);
      ...
    }

##### Chained method calls

    :::C#
    // Bad.
    //   - Line breaks are based on line length, not logic.
    IEnumerable<Module> modules = ImmutableList<Module>.Add(new LifecycleModule())
        .Add(new AppLauncherModule()).AddRange(application.GetModules()).ToBuilder();

    // Better.
    //   - Calls are logically separated.
    //   - However, the trailing period logically splits a statement across two lines.
    IEnumerable<Module> modules = ImmutableList<Module>.
        Add(new LifecycleModule()).
        Add(new AppLauncherModule()).
        AddRange(application.getModules()).
        ToBuilder();

    // Good.
    //   - Method calls are isolated to a line.
    //   - The proper location for a new method call is unambiguous.
    IEnumerable<Module> modules = ImmutableList<Module>
        .Add(new LifecycleModule())
        .Add(new AppLauncherModule())
        .AddRange(application.getModules())
        .ToBuilder();

#### CamelCase for types, camelCase for variables, UPPER_SNAKE for constants

#### No trailing whitespace

Trailing whitespace characters, while logically benign, add nothing to the program.
However, they do serve to frustrate developers when using keyboard shortcuts to navigate code.

### Variable naming

#### Extremely short variable names should be reserved for instances like loop indices.

    :::C#
    // Bad.
    //   - Field names give little insight into what fields are used for.
    class User 
    {
      private int a;
      private string m;

      ...
    }

    // Good.
    class User 
    {
      private int ageInYears;
      private string maidenName;

      ...
    }

#### Include units in variable names

    :::C#
    // Bad.
    long pollInterval;
    int fileSize;

    // Good.
    long pollIntervalMs;
    int fileSizeGb.

#### Don't embed metadata in variable names

A variable name should describe the variable's purpose.  Adding extra information like scope and
type is generally a sign of a bad variable name.

Avoid embedding the field type in the field name.

    :::C#
    // Bad.
    Dictionary<int, User> idToUserDictionary;
    string valueString;

    // Good.
    Dictionary<int, User> usersById;
    string value;

Also avoid embedding scope information in a variable.  Hierarchy-based naming suggests that a class
is too complex and should be broken apart.

    :::C#
    // Bad.
    string _value;
    string mValue;

    // Good.
    string value;

### Implicitly Typed Local Variables [1] [2]

Use [implicit typing](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/implicitly-typed-local-variables) for local variables when the type of the variable is obvious from the right side of the assignment, or when the precise type is not important.

    ::: C#
    // When the type of a variable is clear from the context, use var 
    // in the declaration.
    var var1 = "This is clearly a string.";
    var var2 = 27;
    var var3 = Convert.ToInt32(Console.ReadLine());

Do not use var when the type is not apparent from the right side of the assignment.

    :::C#
    // When the type of a variable is not clear from the context, use an
    // explicit type.
    int var4 = ExampleClass.ResultSoFar();

Do not rely on the variable name to specify the type of the variable. It might not be correct.

    :::C#
    // Naming the following variable inputInt is misleading. 
    // It is a string.
    var inputInt = Console.ReadLine();
    Console.WriteLine(inputInt);

Avoid the use of var in place of [dynamic](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types).

Use implicit typing to determine the type of the loop variable in for and foreach loops.

The following example uses implicit typing in a for statement.

    :::C#
    var phrase = "lalalalalalalalalalalalalalalalalalalalalalalalalalalalalala";
    var manyPhrases = new StringBuilder();
    for (var i = 0; i < 10000; i++)
    {
        manyPhrases.Append(phrase);
    }
    //Console.WriteLine("tra" + manyPhrases);

or

    :::C#
    foreach (var ch in laugh)
    {
        if (ch == 'h')
            Console.Write("H");
        else
            Console.Write(ch);
    }
    Console.WriteLine();

### Unsigned Data Type [1]

In general, use `int` rather than unsigned types. The use of `int` is common throughout C#, and it is easier to interact with other libraries when you use `int`.

### Arrays [1]

Use the concise syntax when you initialize arrays on the declaration line.

    :::C#
    // Preferred syntax. Note that you cannot use var here instead of string[].
    string[] vowels1 = { "a", "e", "i", "o", "u" };


    // If you use explicit instantiation, you can use var.
    var vowels2 = new string[] { "a", "e", "i", "o", "u" };

    // If you specify an array size, you must initialize the elements one at a time.
    var vowels3 = new string[5];
    vowels3[0] = "a";
    vowels3[1] = "e";
    // And so on.

### Delegates [1]

Use the concise syntax to create instances of a delegate type.

    :::C#
    // First, in class Program, define the delegate type and a method that  
    // has a matching signature.

    // Define the type.
    public delegate void Del(string message);

    // Define a method that has a matching signature.
    public static void DelMethod(string str)
    {
        Console.WriteLine("DelMethod argument: {0}", str);
    }

    // In the Main method, create an instance of Del.

    // Preferred: Create an instance of Del by using condensed syntax.
    Del exampleDel2 = DelMethod;

    // The following declaration uses the full syntax.
    Del exampleDel1 = new Del(DelMethod);

### && and || Operators [1]

To avoid exceptions and increase performance by skipping unnecessary comparisons, use && instead of & and || instead of | when you perform comparisons, as shown in the following example.

      :::C#
      Console.Write("Enter a dividend: ");
      var dividend = Convert.ToInt32(Console.ReadLine());

      Console.Write("Enter a divisor: ");
      var divisor = Convert.ToInt32(Console.ReadLine());

      // If the divisor is 0, the second clause in the following condition
      // causes a run-time error. The && operator short circuits when the
      // first expression is false. That is, it does not evaluate the
      // second expression. The & operator evaluates both, and causes 
      // a run-time error when divisor is 0.
      if ((divisor != 0) && (dividend / divisor > 0))
      {
          Console.WriteLine("Quotient: {0}", dividend / divisor);
      }
      else
      {
          Console.WriteLine("Attempted division by 0 ends up here.");
      }

### Static Members [1] [2]

Call [static](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/static) members by using the class name: ClassName.StaticMember. This practice makes code more readable by making static access clear. Do not qualify a static member defined in a base class with the name of a derived class. While that code compiles, the code readability is misleading, and the code may break in the future if you add a static member with the same name to the derived class.

### LINQ Queries [1]

Use meaningful names for query variables. The following example uses seattleCustomers for customers who are located in Seattle.

    :::C#
    var seattleCustomers = from customer in customers
                          where customer.City == "Seattle"
                          select customer.Name;

Use aliases to make sure that property names of anonymous types are correctly capitalized, using Pascal casing.

    :::C#
    var localDistributors =
        from customer in customers
        join distributor in distributors on customer.City equals distributor.City
        select new { Customer = customer, Distributor = distributor };

Rename properties when the property names in the result would be ambiguous. For example, if your query returns a customer name and a distributor ID, instead of leaving them as Name and ID in the result, rename them to clarify that Name is the name of a customer, and ID is the ID of a distributor.

    :::C#
    var localDistributors2 =
        from customer in customers
        join distributor in distributors on customer.City equals distributor.City
        select new { CustomerName = customer.Name, DistributorID = distributor.ID };

Use implicit typing in the declaration of query variables and range variables.

    :::C#
    var seattleCustomers = from customer in customers
                          where customer.City == "Seattle"
                          select customer.Name;

Align query clauses under the from clause, as shown in the previous examples.

Use where clauses before other query clauses to ensure that later query clauses operate on the reduced, filtered set of data.

    :::C#
    var seattleCustomers2 = from customer in customers
                            where customer.City == "Seattle"
                            orderby customer.Name
                            select customer;

Use multiple from clauses instead of a join clause to access inner collections. For example, a collection of Student objects might each contain a collection of test scores. When the following query is executed, it returns each score that is over 90, along with the last name of the student who received the score.

    :::C#
    // Use a compound from to access the inner sequence within each element.
    var scoreQuery = from student in students
                    from score in student.Scores
                    where score > 90
                    select new { Last = student.LastName, score };

### Space pad operators and equals

    :::C#
    // Bad.
    //   - This offers poor visual separation of operations.
    int foo=a+b+1;

    // Good.
    int foo = a + b + 1;

### Be explicit about operator precedence

    :::C#
    // Bad.
    return a << 8 * n + 1 | 0xFF;

    // Good.
    return (a << (8 * n) + 1) | 0xFF;

It's even good to be *really* obvious.

    :::C#
    if ((values != null) && (10 > values.length)) 
    {
      ...
    }

### Documentation

The more visible a piece of code is (and by extension - the farther away consumers might be),
the more documentation is needed.

#### "I'm writing a report about..."

Your elementary school teacher was right - you should never start a statement this way.
Likewise, you shouldn't write documentation this way.

    :::C#
    // Bad.
    /// <summary>
    /// This is a class that implements a cache.  It does caching for you.
    /// </summary>
    class Cache 
    {
      ...
    }

    // Good.
    /// <summary>
    /// A volatile storage for objects based on a key, which may be invalidated and discarded.
    /// </summary>
    class Cache {
      ...
    }

#### Documenting a class

Documentation for a class may range from a single sentence
to paragraphs with code examples. Documentation should serve to disambiguate any conceptual
blanks in the API, and make it easier to quickly and *correctly* use your API.
A thorough class doc usually has a one sentence summary and, if necessary,
a more detailed explanation.

    :::C#
    /// <summary>
    /// An RPC equivalent of a unix pipe tee.  Any RPC sent to the tee input is guaranteed to have
    /// been sent to both tee outputs before the call returns.
    ///
    /// </summary>
    /// <param name="<T>">The type of the tee'd service.</param>
    public class RpcTee<T> 
    {
      ...
    }

#### Documenting a method

A method doc should tell what the method *does*.  Depending on the argument types, it may
also be important to document input format.

    :::C#
    // Bad.
    //   - The doc tells nothing that the method declaration didn't.
    //   - This is the 'filler doc'.  It would pass style checks, but doesn't help anybody.

    /// <summary>
    /// Splits a string.
    /// </summary>
    /// <param name="s">A string.</param>
    /// <returns>
    /// A list of strings.
    /// </returns>
    List<String> split(String s);

    // Better.
    //   - We know what the method splits on.
    //   - Still some undefined behavior.

    /// <summary>
    /// Splits a string on whitespace.
    /// </summary>
    /// <param name="s">The string to split.  An {@code null} string is treated as an empty string.</param>
    /// <returns>
    /// A list of the whitespace-delimited parts of the input.
    /// </returns>
    List<String> split(String s);

    // Great.
    //   - Covers yet another edge case.
    /// <summary>
    /// Splits a string on whitespace.  Repeated whitespace characters are collapsed.
    /// </summary>
    /// <param name="s">The string to split.  An {@code null} string is treated as an empty string.</param>
    /// <returns>
    /// A list of the whitespace-delimited parts of the input.
    /// </returns>
    List<String> split(String s);

#### Be professional

We've all encountered frustration when dealing with other libraries, but ranting about it doesn't
do you any favors.  Suppress the expletives and get to the point.

    :::C#
    // Bad.
    // I hate xml/soap so much, why can't it do this for me!?
    try 
    {
      userId = int.Parse(xml.getField("id"));
    } 
    catch (Exception ex) 
    {
      ...
    }

    // Good.
    // TODO(Jim): Tuck field validation away in a library.
    try 
    {
      userId = int.Parse(xml.getField("id"));
    } 
    catch (Exception ex) 
    {
      ...
    }

#### Don't document overriding methods (usually)

    :::C#
    interface Database 
    {
      /// <summary>
      /// Gets the installed version of the database.
      /// </summary>
      /// <returns>
      /// The database version identifier.
      /// </returns>
      string GetVersion();
    }

    // Bad.
    //   - Overriding method doc doesn't add anything.
    class PostgresDatabase : Database 
    {
      /// <summary>
      /// Gets the installed version of the database.
      /// </summary>
      /// <returns>
      /// The database version identifier.
      /// </returns>
      @Override
      public string GetVersion() 
      {
        ...
      }
    }

    // Good.
    class PostgresDatabase : Database 
    {
      @Override
      public string GetVersion();
    }

    // Great.
    //   - The doc explains how it differs from or adds to the interface doc.
    class dokutiDatabase implements Database 
    {
      /// <summary>
      /// Semantic version number
      /// </summary>
      /// <returns>
      /// The database version in semver format.
      /// </returns>
      @Override
      public string getVersion() 
      {
        ...
      }
    }

#### No author tags

Code can change hands numerous times in its lifetime, and quite often the original author of a source file is irrelevant after several iterations.  It is recommended to rather utilise commit messages instead of leaving author comments in the code.

### Use interfaces

Interfaces decouple functionality from implementation, allowing you to use multiple implementations without changing consumers. Interfaces are a great way to isolate packages - provide a set of interfaces, and keep your implementations package private.

Many small interfaces can seem heavyweight, since you end up with a large number of source files.
Consider the pattern below as an alternative.

    :::C#
    interface FileFetcher 
    {
      File GetFile(string name);

      // All the benefits of an interface, with little source management overhead.
      // This is particularly useful when you only expect one implementation of an interface.
      static class HdfsFileFetcher : FileFetcher 
      {
        @Override File GetFile(string name) 
        {
          ...
        }
      }
    }

#### Leverage or extend existing interfaces

Sometimes an existing interface allows your class to easily 'plug in' to other related classes.
This leads to highly [cohesive](http://en.wikipedia.org/wiki/Cohesion_(computer_science)) code.

    :::C#
    // An unfortunate lack of consideration.  Anyone who wants to interact with Blobs will need to
    // write specific glue code.
    class Blobs 
    {
      byte[] NextBlob() 
      {
        ...
      }
    }

    // Much better.  Now the caller can easily adapt this to standard collections, or do more
    // complex things like filtering.
    class Blobs implements IEnumerable<byte[]> 
    {
      @Override
      Iterator<byte[]> Iterator() 
      {
        ...
      }
    }

Warning - don't bend the definition of an existing interface to make this work.  If the interface doesn't conceptually apply cleanly, it's best to avoid this.

## Writing testable code

Writing unit tests doesn't have to be hard.  You can make it easy for yourself if you keep testability in mind while designing your classes and interfaces.

### Fakes and mocks

When testing a class, you often need to provide some kind of canned functionality as a replacement for real-world behavior.  For example, rather than fetching a row from a real database, you have a test row that you want to return.  This is most commonly performed with a fake object or a mock object.  While the difference sounds subtle, mocks have major benefits over fakes.

    :::C#
    class RpcClient 
    {
      RpcClient(HttpTransport Transport) 
      {
        ...
      }
    }

    // Bad.
    //   - Our test has little control over method call order and frequency.
    //   - We need to be careful that changes to HttpTransport don't disable FakeHttpTransport.
    //   - May require a significant amount of code.
    class FakeHttpTransport : HttpTransport 
    {
      @Override
      void WriteBytes(byte[] bytes) 
      {
        ...
      }

      @Override
      byte[] ReadBytes() 
      {
        ...
      }
    }

    public class RpcClientTest 
    {
      private RpcClient client;
      private FakeHttpTransport transport;

      @Before
      public void SetUp() 
      {
        transport = new FakeHttpTransport();
        client = new RpcClient(transport);
      }

      ...
    }

    interface Transport 
    {
      void WriteBytes(byte[] bytes);
      byte[] ReadBytes();
    }

    class RpcClient 
    {
      RpcClient(Transport transport) 
      {
        ...
      }
    }

    // Good.
    //   - We can mock the interface and have very fine control over how it is expected to be used.
    public class RpcClientTest 
    {
      private RpcClient client;
      private Transport transport;

      @Before
      public void SetUp() 
      {
        transport = Substitute.For<Transport>();
        client = new RpcClient(transport);
      }

      ...
    }

### Let your callers construct support objects

    :::C#
    // Bad.
    //   - A unit test needs to manage a temporary file on disk to test this class.
    class ConfigReader 
    {
      private readonly InputStream configStream;
      ConfigReader(string fileName)
      {
        this.configStream = new FileInputStream(fileName);
      }
    }

    // Good.
    //   - Testing this class is as easy as using ByteArrayInputStream with a String.
    class ConfigReader 
    {
      private readonly InputStream configStream;
      ConfigReader(InputStream configStream)
      {
        this.configStream = checkNotNull(configStream);
      }
    }

### Testing multithreaded code

Testing code that uses multiple threads is notoriously hard.  When approached carefully, however, it can be accomplished without deadlocks or unnecessary time-wait statements.

If you are testing code that needs to perform periodic background tasks
(such as with a [Timer Class](https://docs.microsoft.com/en-us/dotnet/api/system.timers.timer)), consider mocking the service and/or manually triggering the tasks from your tests, and avoiding the actual scheduling.

If you are testing code that performs parallel threaded tasks, rather place the parallel code inside a method, and test that method with a single thread.

### Testing antipatterns

#### Time-dependence

Code that captures real wall time can be difficult to test repeatably, especially when time deltas are meaningful.

#### The hidden stress test

Avoid writing unit tests that attempt to verify a certain amount of performance.  This type of testing should be handled separately, and run in a more controlled environment than unit tests typically are.

#### Thread.Sleep()

Sleeping is rarely warranted, especially in test code.  Sleeping is expressing an expectation that something else is happening while the executing thread is suspended.  This quickly leads to brittleness; for example if the background thread was not scheduled while you were sleeping.

Sleeping in tests is also bad because it sets a firm lower bound on how fast tests can execute. No matter how fast the machine is, a test that sleeps for one second can never execute in less than one second.  Over time, this leads to very long test execution cycles.

### Avoid randomness in tests

Using random values may seem like a good idea in a test, as it allows you to cover more test cases with less code.  The problem is that you lose control over which test cases you're covering.  When you do encounter a test failure, it may be difficult to reproduce.  Pseudorandom input with a fixed seed is slightly better, but in practice rarely improves test coverage.  In general it's better to use fixed input data that exercises known edge cases.

## Best practices

### Defensive programming

#### Avoid assert

Avoid using assert statements since it can be disabled at execution time.

#### Preconditions

Preconditions checks are a good practice, since they serve as a well-defined barrier against bad input from callers.  As a convention, object parameters to public constructors and methods should always be checked against null, unless null is explicitly allowed.

#### Minimize visibility

In a class API, you should support access to any methods and fields that you make accessible.
Therefore, only expose what you intend the caller to use.  This can be imperative when
writing thread-safe code.

    :::C#
    public class Parser 
    {
      // Bad.
      //   - Callers can directly access and mutate, possibly breaking internal assumptions.
      public Dictionary<string, string> rawFields;

      // Bad.
      //   - This is probably intended to be an internal utility function.
      public string ReadConfigLine() 
      {
        ..
      }
    }

    // Good.
    //   - rawFields and the utility function are hidden
    //   - The class is package-private, indicating that it should only be accessed indirectly.
    class Parser 
    {
      private readonly Map<string, string> rawFields;

      private string ReadConfigLine() 
      {
        ..
      }
    }

#### Favor immutability

Mutable objects carry a burden - you need to make sure that those who are *able* to mutate it are not violating expectations of other users of the object, and that it's even safe for them to modify.

    :::C#
    // Bad.
    //   - Anyone with a reference to User can modify the user's birthday.
    //   - Calling getAttributes() gives mutable access to the underlying map.
    public class User 
    {
      public Date birthday;
      private readonly Dictionary<string, string> attributes = new Dictionary<string, string>();

      ...

      public Dictionary<string, string> GetAttributes() 
      {
        return attributes;
      }
    }

    // Good.
    public class User 
    {
      private readonly Date birthday;
      private readonly Dictionary<string, string> attributes = new Dictionary<string, string>();

      ...

      public Map<string, string> getAttributes() 
      {
        return new ReadOnlyDictionary<string, string>(attributes);
      }

      // If you realize the users don't need the full dictionary, you can avoid the dictionary
      // by providing access to individual members.
      public string GetAttribute(string attributeName) 
      {
        return attributes[attributeName];
      }
    }

#### Cleanup unmanaged objects

    :::C#
    // Bad
    //    - Filestream object is never disposed
    void ProcessFile(string path)
    {
      FileStream fs = File.Create(path);

      AddText(fs, "This is some text");
      AddText(fs, "This is some more text,");
    }

    // Good
    //    - FileStream object gets disposed at the end
    void ProcessFile(string path)
    {
      FileStream fs = File.Create(path);

      try
      {
        AddText(fs, "This is some text");
        AddText(fs, "This is some more text,");
      }
      catch (Exception ex)
      {
        ...
      }
      finally
      {
        fs.Dispose();
      }
    }

    // Better
    //    - FileStream object automatically gets disposed at the end of the using statement
    void ProcessFile(string path)
    {
      using (FileStream fs = File.Create(path))
      {
        try
        {
          AddText(fs, "This is some text");
          AddText(fs, "This is some more text,");
        }
        catch (Exception ex)
        {
          ...
        }
      }
    }

### Clean code

#### Disambiguate

Favor readability - if there's an ambiguous and unambiguous route, always favor unambiguous.

#### Remove dead code

Delete unused code (usings, fields, parameters, methods, classes).  They will only rot.

#### Use general types

When declaring fields and methods, it's better to use general types whenever possible.
This avoids implementation detail leak via your API, and allows you to change the types used internally without affecting users or peripheral code.

    :::C#
    // Bad.
    //   - Implementations of Database must match the ArrayList return type.
    //   - Changing return type to Set<User> or List<User> could break implementations and users.
    interface Database 
    {
      ArrayList<User> FetchUsers(string query);
    }

    // Good.
    //   - IEnumerable defines the minimal functionality required of the return.
    interface Database 
    {
      IEnumerable<User> FetchUsers(string query);
    }

#### Clearly defined and appropriately sized classes.

Try to keep classes bite-sized and with clearly-defined responsibilities.  This can be
*really* hard as a program evolves.

Follow the [single responsibility pattern](https://en.wikipedia.org/wiki/Single_responsibility_principle).

#### Avoid typecasting

Typecasting is a sign of poor class design, and can often be avoided.  An obvious exception is overriding [equals](https://docs.microsoft.com/en-us/dotnet/api/system.object.equals?view=netframework-4.8)).

#### Use readonly

*See also [favor immutability](#favor-immutability)*

Readonly is useful because it declares that an object may not be reassigned once initialised.  

#### Avoid mutable static state

Mutable static state is rarely necessary, and causes loads of problems when present.  A very simple case that mutable static state complicates is unit testing.  Since unit tests runs are typically in a single VM, static state will persist through all test cases.  In general, mutable static state is a sign of poor class design.

#### Exceptions

##### Catch narrow exceptions

Sometimes when using try/catch blocks, it may be tempting to just `catch Exception` so you don't have to worry about what type was thrown.  This is usually a bad idea,
as you can end up catching more than you really wanted to deal with.  For example,
`catch Exception` would capture `StorageException`, `NullPointerException` and `OutOfMemoryError`.

    :::C#
    // Bad.
    //   - If a RuntimeException happens, the program continues rather than aborting.
    try 
    {
      storage.InsertUser(user);
    } 
    catch (Exception ex) 
    {
      Log.Error("Failed to insert user.");
    }

    // Good
    //   -  Only StorageExceptions will get logged. Any other Exceptions will be thrown down range.
    try 
    {
      storage.InsertUser(user);
    } 
    catch (StorageException ex) 
    {
      Log.Error("Failed to insert user.");
    }

##### Don't swallow exceptions

An empty `catch` block is usually a bad idea, as you have no signal of a problem.  Coupled with
[narrow exception](#catch-narrow-exceptions) violations, it's a recipe for disaster.

##### Throw appropriate exception types

Let your API users obey [catch narrow exceptions](#catch-narrow-exceptions), don't throw Exception. Even if you are calling another non-conforming API that throws Exception, hide that so it doesn't bubble up even further. 

An effort should also be made to hide implementation details from callers when it comes to exceptions.

### Use newer/better libraries

#### StringBuilder.Append() over string.concat() or string+

StringBuilder.Append() is more performant than string.concat() or string+.

When you modify the StringBuilder, it does not reallocate size for itself until the capacity is reached. When this occurs, the new space is allocated automatically and the capacity is doubled. You can specify the capacity of the StringBuilder class using one of the overloaded constructors.

### Premature optimization is the root of all evil.

Donald Knuth has the [following comments](http://c2.com/cgi/wiki?PrematureOptimization) on the topic.

Unless you have strong evidence that an optimization is necessary, it's usually best to implement the un-optimized version first (possibly leaving notes about where optimizations could be made).

### TODOs

#### Leave TODOs early and often

A TODO isn't a bad thing - it's signaling a future developer (possibly yourself) that a
consideration was made, but omitted for various reasons.  It can also serve as a useful signal when debugging.

#### Leave no TODO unassigned

TODOs should have owners, otherwise they are unlikely to ever be resolved.

    :::C#
    // Bad.
    //   - TODO is unassigned.
    // TODO: Implement request backoff.

    // Good.
    // TODO(George Washington): Implement request backoff.

#### Adopt TODOs

You should adopt an orphan if the owner has left the company/project, or if you make
modifications to the code directly related to the TODO topic.

### Obey the Law of Demeter ([LoD](http://en.wikipedia.org/wiki/Law_of_Demeter))

The Law of Demeter is most obviously violated by breaking the
[one dot rule](http://en.wikipedia.org/wiki/Law_of_Demeter#In_object-oriented_programming), but there are other code structures that lead to violations of the spirit of the law.

#### In classes

Take what you need, nothing more.  This often relates to [clearly defined and appropriately sized classes.](#clearly-defined-and-appropriately-sized-classes), 
but it can also hide in constructors or methods that take few parameters.  The key idea is to defer assembly to the layers of the code that know enough to assemble and instead just take the minimal interface you need to get your work done.

    :::C#
    // Bad.
    //   - Weigher uses hosts and port only to immediately construct another object.
    class Weigher 
    {
      private readonly double defaultInitialRate;

      Weigher(IEnumerable<string> hosts, int port, double defaultInitialRate) 
      {
        this.defaultInitialRate = ValidateRate(defaultInitialRate);
        this.weightingService = CreateWeightingServiceClient(hosts, port);
      }
    }

    // Good.
    class Weigher 
    {
      private readonly double defaultInitialRate;

      Weigher(WeightingService weightingService, double defaultInitialRate) 
      {
        this.defaultInitialRate = ValidateRate(defaultInitialRate);
        this.weightingService = CheckNotNull(weightingService);
      }
    }

If you want to provide a convenience constructor, a factory method or an external factory in the form of a builder you still can, but by making the fundamental constructor of a Weigher only take the things it actually uses it becomes easier to unit-test and adapt as the system involves.

#### In methods

If a method has multiple isolated blocks consider naming these blocks by extracting them
to helper methods that do just one thing.  Besides making the calling sites read less
like code and more like english, the extracted sites are often easier to flow-analyse for human eyes.  The classic case is branched variable assignment.  In the extreme, never do this:

    :::C#
    void Calculate(Subject subject) 
    {
      double weight;
      if (useWeightingService(subject)) 
      {
        try 
        {
          weight = weightingService.weight(subject.id);
        } 
        catch (RemoteException e) 
        {
          throw new LayerSpecificException("Failed to look up weight for " + subject, e)
        }
      } 
      else 
      {
        weight = defaultInitialRate * (1 + onlineLearnedBoost);
      }

      // Use weight here for further calculations
    }

Instead do this:

    :::C#
    void Calculate(Subject subject) 
    {
      double weight = CalculateWeight(subject);

      // Use weight here for further calculations
    }

    private double CalculateWeight(Subject subject) 
    {
      if (useWeightingService(subject)) 
      {
        return FetchSubjectWeight(subject.id)
      } 
      else 
      {
        return CurrentDefaultRate();
      }
    }

    private double FetchSubjectWeight(long subjectId) 
    {
      try 
      {
        return weightingService.weight(subjectId);
      } 
      catch (RemoteException e) 
      {
        throw new LayerSpecificException("Failed to look up weight for " + subject, e)
      }
    }

    private double CurrentDefaultRate() 
    {
      defaultInitialRate * (1 + onlineLearnedBoost);
    }

A code reader that generally trusts methods do what they say can scan calculate
quickly now and drill down only to those methods where I want to learn more.

### Don't Repeat Yourself ([DRY](http://en.wikipedia.org/wiki/Don't_repeat_yourself))

For a more long-winded discussion on this topic, read
[here](http://c2.com/cgi/wiki?DontRepeatYourself).

#### Extract constants whenever it makes sense

#### Centralize duplicate logic in utility functions

### Manage threads properly

When spawning a thread, either directly or with a thread pool, you need to take special care that you properly manage the lifecycle.  

Also ensure that all threads are closed gracefully where possible before application shut down.

### Avoid unnecessary code

#### Superfluous temporary variables

    :::C#
    // Bad.
    //   - The variable is immediately returned, and just serves to clutter the code.
    List<string> strings = fetchStrings();
    return strings;

    // Good.
    return fetchStrings();

#### Unneeded assignment

    :::C#
    // Bad.
    //   - The null value is never realized.
    string value = null;
    try
    {
      value = "The value is " + parse(foo);
    }
    catch (BadException e)
    {
      throw new IllegalStateException(e);
    }

    // Good
    string value;
    try 
    {
      value = "The value is " + parse(foo);
    }
    catch (BadException e)
    {
      throw new IllegalStateException(e);
    }

### The 'fast' implementation

Don't bewilder your API users with a 'fast' or 'optimized' implementation of a method.

    :::C#
    int fastAdd(IEnumerable<int> ints);

    // Why would the caller ever use this when there's a 'fast' add?
    int add(IEnumerable<int> ints);


## References

[1] *C# Coding Conventions (C# Programming Guide)* (2015). Retrieved August 06, 2019, From Microsoft: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions

[2] *C# Reference* (2017). Retrieved August 06, 2019, From Microsoft: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated