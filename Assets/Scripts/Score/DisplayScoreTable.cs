using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


/// <summary>
/// Muestra la tabla de puntuaciones en escena con paginación.
/// Las puntuaciones se ordenan de mayor a menor y se navega entre páginas
/// con el input de navegación horizontal o mediante los botones <see cref="NextPage"/> y <see cref="PreviousPage"/>.
/// </summary>
public class DisplayScoreTable : MonoBehaviour
{
    // ── Servicios ────────────────────────────────────────────────────────────
    /// <summary>Servicio de puntuaciones del que se obtiene la tabla de scores.</summary>
    private IScoreService _scoreService;

    // ── Configuración ────────────────────────────────────────────────────────
    /// <summary>Contenedor donde se instancian las filas de la tabla.</summary>
    public Transform container;

    /// <summary>Prefab de cada fila. Debe tener al menos 3 componentes <see cref="TextMeshProUGUI"/> hijos: nombre, score y pacifist.</summary>
    public GameObject rowPrefab;

    /// <summary>Número de entradas que se muestran por página.</summary>
    public int pageSize = 5;

    /// <summary>Página actual (base 0). Se usa junto a <see cref="pageSize"/> para paginar los resultados.</summary>
    public int currentPage = 0;

    /// <summary>Texto que muestra el número de página actual en formato "actual/total".</summary>
    [SerializeField] private TextMeshProUGUI textoPagina;

    /// <summary>Si es <c>true</c>, llama a <see cref="RefreshTable"/> automáticamente en <see cref="Start"/>.</summary>
    [SerializeField] private bool _RefreshAtStart = false;

    
    // ── Estado interno ───────────────────────────────────────────────────────
    /// <summary>Bloquea la lectura del input de navegación mientras la corrutina de cooldown está activa.</summary>
    private bool _isInputLocked = false;

    /// <summary>Lista de filas instanciadas en el contenedor, para poder destruirlas al refrescar.</summary>
    private List<GameObject> _rows = new List<GameObject>();


    // ── Unity Lifecycle ──────────────────────────────────────────────────────

    /// <summary>
    /// Resuelve el servicio de puntuaciones desde el contenedor de la aplicación.
    /// </summary>
    private void Awake()
    {
        _scoreService = AppContainer.Get<IScoreService>();
    }

    /// <summary>
    /// Refresca la tabla al inicio si <see cref="_RefreshAtStart"/> está activado.
    /// </summary>
    private void Start()
    {
        if (_RefreshAtStart) RefreshTable();
    }
    /// <summary>
    /// Lee el input de navegación horizontal cada frame y cambia de página si no hay cooldown activo.
    /// Aplica un cooldown de 0.5 s tras cada cambio para evitar saltar varias páginas de golpe.
    /// </summary>
    private void Update()
    {
        if (_isInputLocked) return;

        Vector2 navigate = PlayerInputManager.Actions.UI.Navigate.ReadValue<Vector2>();

        if (navigate.x > 0.5f)
        {
            Debug.Log("Has pulsado derecha");
            NextPage();
            StartCoroutine(LockCoroutine());
        }

        if (navigate.x < -0.5f)
        {
            Debug.Log("Has pulsado izquierda");
            PreviousPage();
            StartCoroutine(LockCoroutine());
        }
    }

    // ── Tabla ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Destruye las filas existentes y genera las correspondientes a <see cref="currentPage"/>,
    /// ordenando las puntuaciones de mayor a menor. Actualiza el texto de página si está asignado.
    /// </summary>
    public void RefreshTable()
    {
        foreach (var row in _rows)
            Destroy(row);
        _rows.Clear();

        var sortedScores = _scoreService.getScoreTable().scores
            .OrderByDescending(s => s.score);

        var pageScores = sortedScores
            .Skip(currentPage * pageSize)
            .Take(pageSize);

        foreach (var entry in pageScores)
        {
            GameObject newRow = Instantiate(rowPrefab, container);
            _rows.Add(newRow);

            TextMeshProUGUI[] texts = newRow.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 3)
            {
                texts[0].text = entry.playerName;
                texts[1].text = entry.score.ToString();
                texts[2].text = entry.pacifist.ToString();
            }
        }

        int maxPage = Mathf.CeilToInt((float)sortedScores.Count() / pageSize);
        if (textoPagina != null)
            textoPagina.text = (currentPage + 1) + "/" + maxPage;
    }


    // ── Paginación ───────────────────────────────────────────────────────────

    /// <summary>
    /// Avanza a la página siguiente si no se ha llegado a la última y refresca la tabla.
    /// Llamar desde el evento OnClick de un botón en el Inspector o desde <see cref="Update"/>.
    /// </summary>
    public void NextPage()
    {
        int maxPage = Mathf.CeilToInt((float)_scoreService.getScoreTable().scores.Count / pageSize) - 1;
        if (currentPage < maxPage)
        {
            currentPage++;
            RefreshTable();
        }
    }

    /// <summary>
    /// Retrocede a la página anterior si no se está ya en la primera y refresca la tabla.
    /// Llamar desde el evento OnClick de un botón en el Inspector o desde <see cref="Update"/>.
    /// </summary>
    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            RefreshTable();
        }
    }

    /// <summary>
    /// Bloquea el input de navegación durante 0.5 segundos para evitar cambios de página involuntarios
    /// al mantener pulsada la dirección.
    /// </summary>
    private IEnumerator LockCoroutine()
    {
        _isInputLocked = true;
        yield return new WaitForSecondsRealtime(0.5f);
        _isInputLocked = false;
    }
}