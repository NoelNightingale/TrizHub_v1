namespace TCR.Lib.SysLog
{
    /*
 A detailed explanation of the severity Levels:

DEBUG:
Info useful to developers for debugging the app, not useful during operations
     * 
INFORMATIONAL:
Normal operational messages - may be harvested for reporting, measuring throughput, etc - no action required

NOTICE:
Events that are unusual but not error conditions - might be summarized in an email to developers or admins to spot potential problems - no immediate action required

WARNING:
Warning messages - not an error, but indication that an error will occur if action is not taken, e.g. file system 85% full - each item must be resolved within a given time

ERROR:
Non-urgent failures - these should be relayed to developers or admins; each item must be resolved within a given time

ALERT:
Should be corrected immediately - notify staff who can fix the problem - example is loss of backup ISP connection

CRITICAL:
Should be corrected immediately, but indicates failure in a primary system - fix CRITICAL problems before ALERT - example is loss of primary ISP connection

EMERGENCY:
A "panic" condition - notify all tech staff on call? (earthquake? tornado?) - affects multiple apps/servers/sites...
*/

    public enum Level
    {
        Emergency,
        Alert,
        Critical,
        Error,
        Warning,
        Notice,
        Informational,
        Debug
    }
}