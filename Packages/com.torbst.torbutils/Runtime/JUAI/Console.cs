using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

namespace TorbuTils.JUAI
{
    public class Console : MonoBehaviour
    {
        [Serializable]
        public class Category
        {
            [field: SerializeField] public string Name { get; set; } = "Unnamed";
            [field: SerializeField] public Color BackgroundColor { get; set; } = Color.cyan;
            [field: SerializeField] public Color TextColor { get; set; } = Color.white;
            [field: SerializeField] public float Height { get; set; } = 100f;
        }

        public static Console Instance { get; private set; }

        [Header("CONFIG")]
        [SerializeField] private List<Category> categories = new()
        {
            new() { Name = "Assert", BackgroundColor = Color.green, Height = 100f },
            new() { Name = "Error", BackgroundColor = Color.red, Height = 100f },
            new() { Name = "Exception", BackgroundColor = Color.red, Height = 100f },
            new() { Name = "JUAIConsoleCommand", BackgroundColor = Color.black, Height = 50f },
            new() { Name = "Log", BackgroundColor = Color.gray, Height = 50f },
            new() { Name = "Warning", BackgroundColor = Color.yellow, Height = 80f },
        };
        [SerializeField, Range(0f, 1f)] private float inactiveCategoryButtonOpacity = 0.6f;
        [SerializeField] private KeyCode toggleKey = KeyCode.Tab;
        [SerializeField] private KeyCode sendCommandKey = KeyCode.Return;
        [SerializeField] private KeyCode olderHistoryKey = KeyCode.UpArrow;
        [SerializeField] private KeyCode newerHistoryKey = KeyCode.DownArrow;
        [SerializeField] private bool logUnityDebugDirectly = true;
        [SerializeField] private bool logJUAIConsoleInputDirectly = true;

        [Header("OBJECT REFERENCES")]
        [SerializeField] private GameObject categoryPrefab;
        [SerializeField] private GameObject messagePrefab;
        [SerializeField] private GameObject console;
        [SerializeField] private RectTransform output;
        [SerializeField] private RectTransform categoryButtonsWrapper;
        [SerializeField] private TMP_InputField input;

        private IConsoleActor consoleActor;
        private Dictionary<Category, LogCategoryButton> categoryButtons = new();
        private List<string> JUAIInputHistory = new();
        private int historyIndex = -1;
        private bool ignoreInputChanged = false;

        /// <summary>
        /// Only intended to be used by the JUAI
        /// </summary>
        public void SendJUAIConsoleCommand()
        {
            FocusInput();
            if (input == null)
            {
                Debug.LogError("Can't send command: Console.input is null");
                return;
            }
            string command = input.text;
            // Verify
            bool verified;
            if (consoleActor != null) verified = consoleActor.Validate(command);
            else verified = !string.IsNullOrWhiteSpace(command);
            if (!verified) return;

            if (logJUAIConsoleInputDirectly)
                Log(command, Environment.StackTrace, GetCategory("JUAIConsoleCommand"));
            if (consoleActor != null)
                consoleActor.Execute(command);
            JUAIInputHistory.Add(command);
            historyIndex = -1;
            input.text = "";
        }
        public void JUAIInputChanged()
        {
            if (ignoreInputChanged)
            {
                ignoreInputChanged = false;
                return;
            }
            historyIndex = -1;
        }
        public void CategoryButtonClicked(LogCategoryButton button)
        {
            button.Active = !button.Active;
            float opacity = 1f;
            if (!button.Active)
                opacity = inactiveCategoryButtonOpacity;
            Color bgColor = button.Background.color;
            Color textColor = button.Name.color;
            bgColor.a = opacity;
            textColor.a = opacity;
            button.Background.color = bgColor;
            button.Name.color = textColor;
            FocusInput();

            foreach (var child in output.GetComponentsInChildren<LogMessage>(true))
            {
                bool show = categoryButtons[child.Category].Active;
                child.gameObject.SetActive(show);
            
            }
            output.GetComponent<ListHandler>().Refresh();
        }

        protected virtual GameObject Depool(GameObject go)
        {
            if (EzPools.Pools.Instance == null) return Instantiate(go);
            return EzPools.Pools.Instance.Depool(go);
        }


        private void Awake()
        {
            Instance = this;
            if (categoryButtonsWrapper != null)
            {
                foreach (var category in categories)
                {
                    GameObject go = Depool(categoryPrefab);
                    RectTransform rt = go.GetComponent<RectTransform>();
                    LogCategoryButton button = go.GetComponent<LogCategoryButton>();
                    button.Name.text = category.Name;
                    button.Name.color = category.TextColor;
                    button.Background.color = category.BackgroundColor;
                    categoryButtons.Add(category, button);
                    rt.SetParent(categoryButtonsWrapper);
                }
            }
        }
        private void OnEnable()
        {
            Application.logMessageReceived += HandleUnityLog;
            consoleActor = GetComponent<IConsoleActor>();
            if (consoleActor != null)
                consoleActor.Executed += HandleConsoleActorLog;
        }
        private void OnDisable()
        {
            Application.logMessageReceived -= HandleUnityLog;
            if (consoleActor != null)
                consoleActor.Executed -= HandleConsoleActorLog;
            consoleActor = null;
        }
        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                console.SetActive(!console.activeSelf);
                if (console.activeInHierarchy)
                    FocusInput();
            }
            if (console.activeSelf)
            {
                if (input != null)
                {
                    if (Input.GetKeyDown(sendCommandKey))
                        SendJUAIConsoleCommand();
                }
                bool olderHistoryPressed = Input.GetKeyDown(olderHistoryKey);
                bool newerHistoryPressed = Input.GetKeyDown(newerHistoryKey);
                if (olderHistoryPressed || newerHistoryPressed)
                {
                    if (JUAIInputHistory.Count == 0) return;
                    if (olderHistoryPressed)
                        historyIndex++;
                    if (newerHistoryPressed)
                        historyIndex--;
                    ignoreInputChanged = true;
                    historyIndex = Mathf.Clamp(historyIndex, 0, JUAIInputHistory.Count-1);
                    input.text = JUAIInputHistory[JUAIInputHistory.Count - historyIndex - 1];
                }
            }
        }
        private void FocusInput()
        {
            input.Select();
            input.ActivateInputField();
        }
        private void HandleUnityLog(string log, string stack, LogType type)
        {
            Category category = GetCategory(type.ToString());
            if (logUnityDebugDirectly)
                Log(log, stack, category);
        }
        private void HandleConsoleActorLog(string log, string stack, string categoryString)
        {
            Category category = GetCategory(categoryString);
            Log(log, stack, category);
        }
        private void Log(string log, string stack, Category category)
        {
            if (category == null)
                category = new Category()
                { Name = "NoCategory", BackgroundColor = Color.magenta, TextColor = Color.black };
            GameObject go = Depool(messagePrefab);
            LayoutElement layout = go.GetComponent<LayoutElement>();
            layout.preferredHeight = category.Height;
            LogMessage element = go.GetComponent<LogMessage>();
            go.SetActive(categoryButtons[category].Active);  // before parenting!
            go.GetComponent<RectTransform>().SetParent(output);
            if (element.Message != null)
            {
                element.Message.text = log;
                element.Message.color = category.TextColor;
            }
            if (element.Stack != null)
            {
                element.Stack.text = stack;
                element.Stack.color = category.TextColor;
            }
            if (element.BGColorPanel != null)
                element.BGColorPanel.color = category.BackgroundColor;
            element.Category = category;
        }
        private Category GetCategory(string name)
            => categories.Find(match => match.Name == name);
    }
    public interface IConsoleActor
    {
        public event Action<string, string, string> Executed;
        public bool Validate(string command);
        public bool Execute(string command);
    }
}