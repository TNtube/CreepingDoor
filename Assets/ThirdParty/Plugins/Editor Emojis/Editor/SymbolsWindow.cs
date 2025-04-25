using UnityEditor;
using UnityEngine;
using Anvil.Common;

namespace Anvil.EditorEmojis
{
    /// <summary>
    /// Because we are only going to access emojis through this popup window,
    /// We can use it as a 'container' for dependency injection.
    /// </summary>
    public class SymbolsWindow : PopupWindowContent
    {
        private readonly Vector2 _windowSize = new(300, 270);
        // View
        private readonly SymbolsView _view;
        // Presenter
        private readonly SymbolsPresenter _presenter;

        // GameObject
        private readonly GameObject _gameObject;
        public GameObject GameObject => _gameObject;

        public SymbolsWindow(GameObject gameObject)
        {
            Util.Log("Initializing Symbols Window", LogFilter.Window);
            _gameObject = gameObject;
            _view = new SymbolsView();
            _presenter = new SymbolsPresenter(SymbolsModel.Instance, _view);
        }

        public SymbolsWindow(SerializedObject serializedObject)
            : this(serializedObject.targetObject as GameObject)
        {
            
        }

        public override Vector2 GetWindowSize()
        {
            // Set the size of the popup window
            return _windowSize;
        }

        public override void OnGUI(Rect rect)
        {

        }

        public override void OnOpen()
        {
            Util.Log("Opening Symbols Window", LogFilter.Window);
            editorWindow.rootVisualElement.Add(_view);
            SymbolsModel.Instance.OnSymbolClicked += OnSymbolClicked;
            _presenter.OnOpen();
        }

        public override void OnClose()
        {
            Util.Log("Closing Symbols Window", LogFilter.Window);
            editorWindow.rootVisualElement.Remove(_view);
            SymbolsModel.Instance.OnSymbolClicked -= OnSymbolClicked;
            _presenter.Dispose();
        }

        private void OnSymbolClicked(Symbol symbol)
        {
            Util.Log($"Clicked symbol: {symbol.Name}. Loading it's serialized texture into to GO {GameObject.name}", LogFilter.Window);
            var serializedIconTexture = symbol.GetSerializedTexture();
            EditorGUIUtility.SetIconForObject(GameObject, serializedIconTexture);
            editorWindow.Close();
        }
    }
}