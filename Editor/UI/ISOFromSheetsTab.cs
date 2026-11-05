using UnityEngine;

namespace ScriptableObjectsFromSheets.UI
{
    public interface ISOFromSheetsTab
    {
        string TabName { get; }
        void OnEnable();
        void OnDisable();
        void OnGUI(Rect contentRect);
    }
}