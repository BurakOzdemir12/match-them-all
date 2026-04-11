using _Project.Scripts.Lobby.Data.Save;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;

namespace _Project.Scripts.Lobby.Structs
{
    [System.Serializable]
    public struct SavedPartData
    {
        public PlanePartSo partSo;
        public PartSaveInfo saveInfo;

        public SavedPartData(PlanePartSo partSo, PartSaveInfo saveInfo)
        {
            this.partSo = partSo;
            this.saveInfo = saveInfo;
        }
    }
}