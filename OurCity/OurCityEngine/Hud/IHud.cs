using System;
namespace OurCityEngine.Hud {
    /// <summary>
    /// Hud interface
    /// </summary>
    public interface IHud {
        void AddMainMessage(string message); 
        void AddObjective(Message message);
        void RemoveObjective(Message message);
        void SetObjective(Message message);
        void Notify(Message message);
        MinimapPoi AddMinimapPoi(Microsoft.Xna.Framework.Vector3 position, Microsoft.Xna.Framework.Color color, PoiShape shape = PoiShape.Square);
    }
}
