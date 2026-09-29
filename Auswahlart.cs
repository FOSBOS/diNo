namespace diNo
{
  // Ursprünglich in Datenauswahl.cs (WinForms) zusammen mit der inzwischen durch
  // DatenauswahlWindow/DatenauswahlViewModel ersetzten Form definiert. Wird weiterhin von
  // Zugriff.cs (selectedAuswahlart) und den Report-/Administration-Viewmodels gebraucht.
  public enum Auswahlart
  {
    Vorkommnis,
    Zubringerschule,
    Wiederholer,
    Probezeit,
    Fremdsprache2,
    Abschluss // Hier werden alle links selektierten Schüler gedruckt.
  }
}
