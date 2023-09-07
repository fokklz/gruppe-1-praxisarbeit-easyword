# easyword-Vorlage

Um dieses Repository zu verwenden, erstelle einfach einen `Fork` davon.

Als nächstes ist es notwendig, einen neuen Access-Token zu generieren, der über die erforderlichen Berechtigungen verfügt, um den Job auszuführen.

[Neuen spezifischen Access-Token erstellen](https://github.com/settings/personal-access-tokens/new)
Der Token sollte folgende Berechtigungen haben:
   ✓ Lesezugriff auf Metadaten
   ✓ Lese- und Schreibzugriff auf Aktionen und Inhalte (Actions & Contents)

Füge diesen Token in den Einstellungen deines Repositories hinzu:

`Secrets and variables` > `Actions`
  Dort kannst du ein Secret erstellen.
  Benenne es `G_PAT` und füge den soeben generierten Token ein.

**FERTIG**
Bei jedem Push, bei dem sich die `VERSION.txt` ändert, wird automatisch ein neuer Build inklusive Release erstellt.
