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
using System.Globalization;

namespace Elevate
{
    /// <summary>
    /// The command line argument parsing class.
    /// </summary>
    internal class ElevateArgParser
    {
        private string [] rawArgs;
        StringBuilder argBuilder;

        /// <summary>
        /// Constructs the class.
        /// </summary>
        /// <param name="args">
        /// The command line arguments passed to the applicationToRun.
        /// </param>
        public ElevateArgParser ( string [] args )
        {
            rawArgs = args;
            ParseError = String.Empty;
            argBuilder = new StringBuilder ( );
        }

        private Boolean showHelp;
        /// <summary>
        /// True if help was requested.
        /// </summary>
        public Boolean ShowHelp
        {
            get { return showHelp; }
            set { showHelp = value; }
        }

        private Boolean useComSpecEnvironment;
        /// <summary>
        /// True if using the %comspec% environment value and keeping the 
        /// elevated command prompt open.
        /// </summary>
        public Boolean UseComSpecEnvironment
        {
            get { return useComSpecEnvironment; }
            set { useComSpecEnvironment = value; }
        }

        private Boolean waitForTermination;
        /// <summary>
        /// True if Elevate is supposed to wait for termination.
        /// </summary>
        public Boolean WaitForTermination
        {
            get { return waitForTermination; }
            set { waitForTermination = value; }
        }

        private String parseError;
        /// <summary>
        /// The parsing error to report.
        /// </summary>
        public String ParseError
        {
            get { return parseError; }
            set { parseError = value; }
        }

        private String applicationToRun;
        /// <summary>
        /// The applicationToRun to execute.
        /// </summary>
        public String ApplicationToRun
        {
            get { return applicationToRun; }
            set { applicationToRun = value; }
        }

        /// <summary>
        /// Arguments to the program to run.
        /// </summary>
        public String Arguments
        {
            get { return ( argBuilder.ToString ( ).Trim ( ) ); }
        }

        /// <summary>
        /// Parses up the argument string.
        /// </summary>
        /// <returns>
        /// True if life is happy.
        /// </returns>
        public Boolean ParseArguments ( )
        {
            // Do the easy check.
            if ( 0 == rawArgs.Length )
            {
                ShowHelp = true;
                return ( false );
            }

            // Have we seen the applicationToRun?
            Boolean seenProgramArg = false;
            // The current index in the array.
            int currentIndex = 0;
            while ( currentIndex < rawArgs.Length )
            {
                // Check to see if this is an Elevate argument.
                if ( false == seenProgramArg )
                {
                    String elevateArg = rawArgs [ currentIndex ] ;
                    if ( ( '-' == elevateArg [ 0 ] ) || ( '/' == elevateArg [ 0 ] ) )
                    {
                        elevateArg = elevateArg.Substring ( 1 );
                        if ( 0 == String.Compare ( elevateArg , 
                                                   Constants.HelpArg , 
                                                   true , 
                                                  CultureInfo.CurrentCulture ) )
                        {
                            ShowHelp = true;
                            return ( false );
                        }
                        else if ( 0 == String.Compare ( elevateArg ,
                                                        Constants.KeepArg ,
                                                        true ,
                                                  CultureInfo.CurrentCulture ) )
                        {
                            UseComSpecEnvironment = true;
                        }
                        else if ( 0 == String.Compare ( elevateArg ,
                                                        Constants.WaitArg ,
                                                        true ,
                                                  CultureInfo.CurrentCulture ) )
                        {
                            WaitForTermination = true;
                        }
                        else
                        {
                            ParseError = String.Format (
                                                CultureInfo.CurrentCulture ,
                                                Constants.InvalidElevateArgFmt ,
                                                elevateArg );
                            return ( false );
                        }
                    }
                    else
                    {
                        seenProgramArg = true;

                        // Got our applicationToRun.
                        ApplicationToRun = rawArgs [ currentIndex ];
                    }
                }
                else
                {
                    // We're doing arguments at this point.
                    String currArg = rawArgs [ currentIndex ] ;
                    String fmt = "{0} ";
                    if ( true == currArg.Contains ( " " ) )
                    {
                        fmt = "\"{0}\" ";
                    }
                    argBuilder.AppendFormat ( fmt , currArg ) ;
                }
                // Prepare to do the next arg.
                currentIndex++;
            }

            // If the program is empty, it's an error.
            if ( true == String.IsNullOrEmpty ( ApplicationToRun ) )
            {
                ShowHelp = true;
                return ( false );
            }
            return ( true );
        }
    }
}
