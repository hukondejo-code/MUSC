MU SERVER CONTROL
Complete private server manager tool designed for MU ONLINE but can be used with any other servers because of its modularity.







Changes:
feat(settings): INI-backed dynamic settings, per-row config and VAR
•	Replace Settings.dat with Settings.ini and add migration

•	Rework Form2 to dynamic rows with Browse/Cfg/Open, in-row VAR textbox (validated, min=1)

•	Add Delays tab and keep delays synced with rows; reindex on removal and persist VAR_n

•	Update Form1 to create dynamic tabs, track embedded processes, and graceful shutdown

•	Remove broken designer resource references and add safe fallbacks
