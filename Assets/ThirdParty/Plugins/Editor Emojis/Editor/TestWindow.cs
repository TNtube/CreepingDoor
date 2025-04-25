using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Anvil.EditorEmojis
{
    internal class TestWindow : EditorWindow
    {
        private SymbolsView _symbolsView;
        private SymbolsModel _symbolsModel;
        private SymbolsPresenter _symbolsPresenter;

        //[MenuItem("Window/Test Emoji Window")]
        public static void ShowWindow()
        {
            GetWindow<TestWindow>("Test Emoji Window");
        }

        private void OnEnable()
        {
            _symbolsModel = new SymbolsModel();
            _symbolsView = new SymbolsView();
            _symbolsPresenter = new SymbolsPresenter(_symbolsModel, _symbolsView);
            rootVisualElement.Add(_symbolsView);
        }
    }
}

