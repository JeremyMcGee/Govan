Govan
=====

Builds .NET servers on AWS.

Given an XML server layout, Govan configures AWS server instances to match. 

TODO
====
It's early days for Govan, so there's nothing but a task list yet. 

- Create format for "environment manifest"
- What kind of "UI"? (I think a command line might be best - can then wrap with Jenkins etc.)
- Create first command: "apply" to apply a server layout to an environment
- Handle first layout element: "name" to apply a server name
- Work out a way to "bootstrap" getting security access
- Und so weiter.

Connecting to the remote box? Options include:

- Powershell remoting
- WMI
- Raw PSEXEC

Principal advantage of PSEXEC is that we can run anything, e.g. Chocolately. Can we do the same with remote Powershell?
Remote Powershell is simpler if we can get it to work, not least as it pipelines output back to the host machine.
However, may not work properly for machines that don't have remote Powershell enabled: could need jiggery-pokery
with PSEXEC to get it started. (update: doesn't seem easy to do, need to override execution policy...)

(Overriding execution policy: looks like the registry key HKLM\Software\Policies\Microsoft\Windows\Powershell!EnableScripts DWORD 1 / !ExecutionPolicy SZ Unrestricted


Command structure:

govan prep  -- prepare the local box for Govan access through Powershell remoting
