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
- Und so weiter.

Connecting to the remote box? Options include:

- Powershell remoting
- WMI
- Raw PSEXEC

Principal advantage of PSEXEC is that we can run anything, e.g. Chocolately. Can we do the same with remote Powershell?
Remote Powershell is simpler if we can get it to work, not least as it pipelines output back to the host machine.
