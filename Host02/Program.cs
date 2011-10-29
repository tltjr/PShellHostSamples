using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Globalization;

namespace Host02
{
    class Host02
  {
    /// <summary>
    /// Gets or sets a Boolean value that the PSHost implementation 
    /// uses to indicate that the host application should exit.
    /// </summary>
    public bool ShouldExit
    {
      get { return shouldExit; }
      set { shouldExit = value; }
    }
    private bool shouldExit;
    
    /// <summary>
    /// Gets or sets a value that the PSHost implementation uses 
    /// this property to indicate which code the host application 
    /// should use when exiting.
    /// </summary>
    public int ExitCode
    {
      get { return exitCode; }
      set { exitCode = value; }
    }
    private int exitCode;
    
    /// <summary>
    /// This sample uses a PowerShell object with a host
    /// implementation to call the Get-Process cmdlet and 
    /// display the results as you would see them when using 
    /// pwrsh.exe.
    /// </summary>
    /// <param name="args">Not used.</param>
    static void Main(string[] args)
    {
      // Set the current culture to German. We want this to be picked up when the MyHost
      // instance is created.
      System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-de");
    
      // Create a runspace that uses the host object.
      MyHost myHost = new MyHost(new Host02());
      using (Runspace myRunSpace = RunspaceFactory.CreateRunspace(myHost))
      {
        myRunSpace.Open();
      
        // Create a PowerShell object to run the commands.
        using (PowerShell powershell = PowerShell.Create())
        {
          powershell.Runspace = myRunSpace;
        
          // Add the script to run. This script does two things. The  
          // Get-Process cmdlet is piped to sort (alias for Sort-Object) 
          // to display the processes by their handle count, and then 
          // the Get-Date cmdlet is piped to Out-String so that the date 
          // date is displayed German.
        
          powershell.AddScript(@"
                               get-process | sort handlecount
                               # This should display the date in German.
                               get-date | out-string");
          
          // Add the default outputter to the end of the pipe and then call the 
          // MergeMyResults method to merge the output and error streams from the 
          // pipeline. This will result in the output being written using the PSHost
          // and PSHostUserInterface classes instead of returning objects to the host
          // application.
        
          powershell.AddCommand("out-default");
          powershell.Commands.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
        
          // Invoke the commands in the pipeline. Nothing more is needed because 
          // no objects are returned. They are all consumed by the Out-Default 
          // cmdlet.

          powershell.Invoke();
        }
      }
    
      System.Console.WriteLine("Hit any key to exit...");
      System.Console.ReadKey();
    }
  }
}
