using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI 요소 - Inspector 에서 연결")]
    public GameObject DialoguePanel;             // 대화창 전체 패널
    public Image characterImage;                 // 캐릭터 이미지
    public TextMeshProUGUI characterNameText;    // 캐릭터 이름 텍스트
    public TextMeshProUGUI dialogueText;         // 대화 내용 표시 텍스트
    public Button nextButton;                    // 다음 버튼

    [Header("기본 설정")]
    public Sprite defaultCharacterImage;         // 캐릭터 기본 이미지 (null일 때 사용)

    [Header("타이핑 효과 설정")]
    public float typingSpeed = 0.05f;            // 글자 하나당 출력 속도
    public bool skipTypingOnClick = true;        // 클릭 시 타이핑 즉시 완료 여부

    // ───────────────────────────────────────────────────
    // 내부 변수들
    private DialogueDataSO currentDialogue;      // 현재 진행 중인 대화 데이터
    private int currentLineIndex = 0;            // 현재 몇 번째 대화 중인지 (0부터 시작)
    private bool isDialogueActive = false;       // 대화 진행 중인지 확인하는 플래그
    private bool isTyping = false;               // 현재 타이핑 효과가 진행 중인지 확인
    private Coroutine typingCoroutine;           // 타이핑 효과 코루틴 참조 (중지용)

    // Start is called before the first frame update
    void Start()
    {
        // 처음에 대화창 숨기기
        DialoguePanel.SetActive(false);
        // "다음" 버튼에 입력 처리 연결
        nextButton.onClick.AddListener(HandleNextInput);
    }

    // Update is called once per frame
    void Update()
    {
        // 스페이스바나 버튼 클릭 시 처리
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            HandleNextInput();   // 다음 입력 처리 (타이핑 중이면 완료, 아니면 다음 줄)
        }
    }

    /// <summary>
    /// 새로운 대화를 시작하는 함수
    /// Inspector에서 만든 DialogueDataSO 에셋을 인자로 넘김
    /// </summary>
    public void StartDialogue(DialogueDataSO dialogue)
    {
        if (dialogue == null || dialogue.dialogueLines.Count == 0)
            return;   // 대화 데이터가 없거나 대화 내용이 비어있으면 실행하지 않음

        // 대화 시작 준비
        currentDialogue = dialogue;          // 현재 대화 데이터 설정
        currentLineIndex = 0;                // 첫 번째 대화부터 시작
        isDialogueActive = true;             // 대화 활성화 플래그 ON

        // UI 업데이트
        DialoguePanel.SetActive(true);       // 대화창 보이기
        characterNameText.text = dialogue.characterName;  // 캐릭터 이름 표시

        if (characterImage != null)
        {
            if (dialogue.characterImage != null)
                characterImage.sprite = dialogue.characterImage;  // 대화 데이터의 이미지 사용
            else
                characterImage.sprite = defaultCharacterImage;     // 기본 이미지 사용
        }

        ShowCurrentLine();   // 첫 번째 대화 내용 표시
    }

    /// <summary>
    /// 현재 대화 줄의 내용을 타이핑 효과와 함께 화면에 표시하는 함수
    /// </summary>
    public void ShowCurrentLine()
    {
        if (currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count)
        {
            // 이전 타이핑 효과 중지
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            // 현재 줄 대사 가져오기
            string currentText = currentDialogue.dialogueLines[currentLineIndex];
            // 타이핑 코루틴 시작
            typingCoroutine = StartCoroutine(TypeText(currentText));
        }
    }

    /// <summary>
    /// 타이핑 효과 전체 로직 (글자 하나씩 추가하면서 대화창에 출력)
    /// </summary>
    IEnumerator TypeText(string textToType)
    {
        isTyping = true;           // 타이핑 시작
        dialogueText.text = "";    // 텍스트 초기화

        for (int i = 0; i < textToType.Length; i++)
        {
            dialogueText.text += textToType[i];             // 한 글자씩 추가
            yield return new WaitForSeconds(typingSpeed);   // 대기 시간 설정
        }

        isTyping = false;          // 타이핑 완료
    }

    /// <summary>
    /// 타이핑 효과를 즉시 완료시키는 함수
    /// 남은 텍스트를 한 번에 화면에 표시
    /// </summary>
    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);  // 코루틴 중지
            typingCoroutine = null;
        }

        isTyping = false;  // 타이핑 상태 해제

        // 현재 줄의 전체 텍스트 즉시 표시
        if (currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count)
            dialogueText.text = currentDialogue.dialogueLines[currentLineIndex];
    }

    /// <summary>
    /// 다음 대화 줄로 이동시키는 함수 (타이핑이 완료된 후에만 호출)
    /// </summary>
    public void ShowNextLine()
    {
        currentLineIndex++;   // 다음 줄로 인덱스 증가

        if (currentLineIndex >= currentDialogue.dialogueLines.Count)
        {
            EndDialogue();     // 마지막 대화였는지 확인 후 대화 종료
        }
        else
        {
            ShowCurrentLine(); // 대화가 남아있으면 다음 줄 표시
        }
    }

    /// <summary>
    /// 스페이스바나 버튼 클릭 시 호출되는 입력 처리 함수
    /// 타이핑 중이면 즉시 완료, 아니면 다음 줄 표시
    /// </summary>
    public void HandleNextInput()
    {
        if (isTyping && skipTypingOnClick)
        {
            CompleteTyping();  // 타이핑 중이라면 즉시 완료
        }
        else if (!isTyping)
        {
            ShowNextLine();    // 타이핑이 완료된 상태면 다음 줄 표시
        }
    }

    /// <summary>
    /// 대화 전체를 바로 스킵하는 함수
    /// </summary>
    public void SkipDialogue()
    {
        EndDialogue();
    }

    /// <summary>
    /// 현재 대화가 진행 중인지 확인하는 함수
    /// </summary>
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    /// <summary>
    /// 대화를 완전히 종료하는 함수
    /// </summary>
    void EndDialogue()
    {
        // 타이핑 효과 정리
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isDialogueActive = false;      // 대화창 비활성화
        isTyping = false;              // 타이핑 상태 해제
        DialoguePanel.SetActive(false); // 대화창 숨기기
        currentLineIndex = 0;          // 인덱스 초기화
        currentDialogue = null;        // 대화 데이터 초기화
    }
}
