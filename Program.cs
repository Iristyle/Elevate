/*------------------------------------------------------------------------------
 * Wintellect Mastering .NET Debugging
 * Copyright © 2007 John Robbins -- All rights reserved. 
 * 
 * A applicationToRun that you can run another applicationToRun from the command line with 
 * elevated rights under Vista.
 -----------------------------------------------------------------------------*/
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace Elevate
{
    class Program
    {
        static void Main ( string [] args )
        {
            ElevateArgParser argData = new ElevateArgParser ( args );
            if ( true == argData.ParseArguments ( ) )
            {
                String progToRun = argData.ApplicationToRun;
                String progArgs = argData.Arguments;

                if ( true == argData.UseComSpecEnvironment )
                {
                    if ( true == progToRun.Contains ( " " ) )
                    {
                        progToRun = String.Format ( CultureInfo.CurrentCulture ,
                                                    "\"{0}\"" ,
                                                    progToRun );
                    }
                    // Add the program as an argument.
                    progArgs = String.Format ( CultureInfo.CurrentCulture , 
                                               "{0} /k {1}" ,
                                               progToRun ,
                                               progArgs );
                    progToRun = Environment.GetEnvironmentVariable (
                                                                    "ComSpec" );
                }

                ProcessStartInfo psInfo = new ProcessStartInfo ( );
                psInfo.Arguments = progArgs;
                psInfo.FileName = progToRun;
                psInfo.UseShellExecute = true;

                if ( true == IsVistaOrBetter ( ) )
                {
                    psInfo.Verb = "runas";
                }
                try
                {
                    Process p = Process.Start ( psInfo );
                    if ( true == argData.WaitForTermination )
                    {
                        p.WaitForExit ( );
                    }
                }
                catch ( Win32Exception ex )
                {
                    Console.WriteLine ( ex.Message );
                }

            }
            else
            {
                ShowHelp ( );
                if ( false == argData.ShowHelp )
                {
                    Console.WriteLine ( argData.ParseError );
                }
            }
        }

        static Boolean IsVistaOrBetter ( )
        {
            if ( ( Environment.OSVersion.Version.Major >= 6 ) &&
                 ( Environment.OSVersion.Version.Minor >= 0 ) )
            {
                return ( true );
            }
            return ( false );
        }

        static void ShowHelp ( )
        {
            // Get the EXE module.
            ProcessModule exe = Process.GetCurrentProcess ( ).Modules [ 0 ];
            Console.WriteLine ( Constants.HelpInfo ,
                                exe.FileVersionInfo.FileVersion );
        }
    }

}
