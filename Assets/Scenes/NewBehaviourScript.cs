using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI ��� - Inspector ���� ����")]
    public GameObject DialoguePanel;             // ��ȭâ ��ü �г�
    public Image characterImage;                 // ĳ���� �̹���
    public TextMeshProUGUI characterNameText;    // ĳ���� �̸� �ؽ�Ʈ
    public TextMeshProUGUI dialogueText;         // ��ȭ ���� ǥ�� �ؽ�Ʈ
    public Button nextButton;                    // ���� ��ư

    [Header("�⺻ ����")]
    public Sprite defaultCharacterImage;         // ĳ���� �⺻ �̹��� (null�� �� ���)

    [Header("Ÿ���� ȿ�� ����")]
    public float typingSpeed = 0.05f;            // ���� �ϳ��� ��� �ӵ�
    public bool skipTypingOnClick = true;        // Ŭ�� �� Ÿ���� ��� �Ϸ� ����

    // ������������������������������������������������������������������������������������������������������
    // ���� ������
    private DialogueDataSO currentDialogue;      // ���� ���� ���� ��ȭ ������
    private int currentLineIndex = 0;            // ���� �� ��° ��ȭ ������ (0���� ����)
    private bool isDialogueActive = false;       // ��ȭ ���� ������ Ȯ���ϴ� �÷���
    private bool isTyping = false;               // ���� Ÿ���� ȿ���� ���� ������ Ȯ��
    private Coroutine typingCoroutine;           // Ÿ���� ȿ�� �ڷ�ƾ ���� (������)

    // Start is called before the first frame update
    void Start()
    {
        // ó���� ��ȭâ �����
        DialoguePanel.SetActive(false);
        // "����" ��ư�� �Է� ó�� ����
        nextButton.onClick.AddListener(HandleNextInput);
    }

    // Update is called once per frame
    void Update()
    {
        // �����̽��ٳ� ��ư Ŭ�� �� ó��
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            HandleNextInput();   // ���� �Է� ó�� (Ÿ���� ���̸� �Ϸ�, �ƴϸ� ���� ��)
        }
    }

    /// <summary>
    /// ���ο� ��ȭ�� �����ϴ� �Լ�
    /// Inspector���� ���� DialogueDataSO ������ ���ڷ� �ѱ�
    /// </summary>
    public void StartDialogue(DialogueDataSO dialogue)
    {
        if (dialogue == null || dialogue.dialogueLines.Count == 0)
            return;   // ��ȭ �����Ͱ� ���ų� ��ȭ ������ ��������� �������� ����

        // ��ȭ ���� �غ�
        currentDialogue = dialogue;          // ���� ��ȭ ������ ����
        currentLineIndex = 0;                // ù ��° ��ȭ���� ����
        isDialogueActive = true;             // ��ȭ Ȱ��ȭ �÷��� ON

        // UI ������Ʈ
        DialoguePanel.SetActive(true);       // ��ȭâ ���̱�
        characterNameText.text = dialogue.characterName;  // ĳ���� �̸� ǥ��

        if (characterImage != null)
        {
            if (dialogue.characterImage != null)
                characterImage.sprite = dialogue.characterImage;  // ��ȭ �������� �̹��� ���
            else
                characterImage.sprite = defaultCharacterImage;     // �⺻ �̹��� ���
        }

        ShowCurrentLine();   // ù ��° ��ȭ ���� ǥ��
    }

    /// <summary>
    /// ���� ��ȭ ���� ������ Ÿ���� ȿ���� �Բ� ȭ�鿡 ǥ���ϴ� �Լ�
    /// </summary>
    public void ShowCurrentLine()
    {
        if (currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count)
        {
            // ���� Ÿ���� ȿ�� ����
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            // ���� �� ��� ��������
            string currentText = currentDialogue.dialogueLines[currentLineIndex];
            // Ÿ���� �ڷ�ƾ ����
            typingCoroutine = StartCoroutine(TypeText(currentText));
        }
    }

    /// <summary>
    /// Ÿ���� ȿ�� ��ü ���� (���� �ϳ��� �߰��ϸ鼭 ��ȭâ�� ���)
    /// </summary>
    IEnumerator TypeText(string textToType)
    {
        isTyping = true;           // Ÿ���� ����
        dialogueText.text = "";    // �ؽ�Ʈ �ʱ�ȭ

        for (int i = 0; i < textToType.Length; i++)
        {
            dialogueText.text += textToType[i];             // �� ���ھ� �߰�
            yield return new WaitForSeconds(typingSpeed);   // ��� �ð� ����
        }

        isTyping = false;          // Ÿ���� �Ϸ�
    }

    /// <summary>
    /// Ÿ���� ȿ���� ��� �Ϸ��Ű�� �Լ�
    /// ���� �ؽ�Ʈ�� �� ���� ȭ�鿡 ǥ��
    /// </summary>
    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);  // �ڷ�ƾ ����
            typingCoroutine = null;
        }

        isTyping = false;  // Ÿ���� ���� ����

        // ���� ���� ��ü �ؽ�Ʈ ��� ǥ��
        if (currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count)
            dialogueText.text = currentDialogue.dialogueLines[currentLineIndex];
    }

    /// <summary>
    /// ���� ��ȭ �ٷ� �̵���Ű�� �Լ� (Ÿ������ �Ϸ�� �Ŀ��� ȣ��)
    /// </summary>
    public void ShowNextLine()
    {
        currentLineIndex++;   // ���� �ٷ� �ε��� ����

        if (currentLineIndex >= currentDialogue.dialogueLines.Count)
        {
            EndDialogue();     // ������ ��ȭ������ Ȯ�� �� ��ȭ ����
        }
        else
        {
            ShowCurrentLine(); // ��ȭ�� ���������� ���� �� ǥ��
        }
    }

    /// <summary>
    /// �����̽��ٳ� ��ư Ŭ�� �� ȣ��Ǵ� �Է� ó�� �Լ�
    /// Ÿ���� ���̸� ��� �Ϸ�, �ƴϸ� ���� �� ǥ��
    /// </summary>
    public void HandleNextInput()
    {
        if (isTyping && skipTypingOnClick)
        {
            CompleteTyping();  // Ÿ���� ���̶�� ��� �Ϸ�
        }
        else if (!isTyping)
        {
            ShowNextLine();    // Ÿ������ �Ϸ�� ���¸� ���� �� ǥ��
        }
    }

    /// <summary>
    /// ��ȭ ��ü�� �ٷ� ��ŵ�ϴ� �Լ�
    /// </summary>
    public void SkipDialogue()
    {
        EndDialogue();
    }

    /// <summary>
    /// ���� ��ȭ�� ���� ������ Ȯ���ϴ� �Լ�
    /// </summary>
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    /// <summary>
    /// ��ȭ�� ������ �����ϴ� �Լ�
    /// </summary>
    void EndDialogue()
    {
        // Ÿ���� ȿ�� ����
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isDialogueActive = false;      // ��ȭâ ��Ȱ��ȭ
        isTyping = false;              // Ÿ���� ���� ����
        DialoguePanel.SetActive(false); // ��ȭâ �����
        currentLineIndex = 0;          // �ε��� �ʱ�ȭ
        currentDialogue = null;        // ��ȭ ������ �ʱ�ȭ
    }
}
