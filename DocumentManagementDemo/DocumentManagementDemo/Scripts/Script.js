// FileManager
function applyFilter(name, text) {
    var filterArg = name === 'All' ? '' : name + '|' + text;
    fileManager.PerformCallback(filterArg);
};
function onFiltersMenuItemClick(s, e) {
    var filterName = e.item.name;
    var filterText = e.item.GetText();

    if (leftPanel.IsExpandable() && leftPanel.IsExpanded())
        leftPanel.Collapse();

    setTimeout(function () { // Timeout - trick for Mobile - wait animation complete on Mobile
        applyFilter(filterName, filterText);
    }, 100);

    // History API
    var kind = filterName === 'All' ? 'path' : 'filter';
    PushToHistoryState(kind, filterName, filterText, '');
    skipUpdateHistoryState = false;
}

// Adaptive
function toggleLeftPanel() {
    if (leftPanel.IsExpandable())
        leftPanel.Toggle();
    else {
        leftPanel.SetVisible(!leftPanel.GetVisible());
        fileManager.AdjustControl();
    }
}

// FileManager ItemTemplate
function openCurrentFolder() {
    this.fileManager.ExecuteCommand(ASPxClientFileManagerCommandConsts.Open);
};
function onFileManagerSelectedFileOpened(s, e) {
    window.open("OpenDocumentHandler.aspx?id=" + e.file.id);
};

function updateFileManagerFiltered(hasFilter) {
    if (hasFilter)
        ASPx.AddClassNameToElement(fileManager.GetMainElement(), "filtered");
    else
        ASPx.RemoveClassNameFromElement(fileManager.GetMainElement(), "filtered");
};
function onFileManagerInit(s, e) {
    updateFileManagerFiltered(fileManager.cpHasFilter);
};
function onFileManagerEndCallback(s, e) {
    updateFileManagerFiltered(fileManager.cpHasFilter);
};

// HistoryApi
var skipUpdateHistoryState = false;

function initHistoryState() {
    history.replaceState({ kind: 'path', filter: '', text: '', path: '' }, 'Documents');
    window.addEventListener('popstate', onPopHistoryState);
};

function PushToHistoryState(kind, filterName, filterText, path) {
    history.pushState({ kind: kind, filter: filterName, text: filterText, path: path }, 'Documents');
}

function onFileManagerCurrentFolderChanged(s, e) {
    if (!skipUpdateHistoryState)
        PushToHistoryState('path', '', '', s.GetCurrentFolderPath('/', true));
    skipUpdateHistoryState = false;
}

function onPopHistoryState(evt) {
    skipUpdateHistoryState = true; // Prevent repeated push to state in onFileManagerCurrentFolderChanged
    var state = evt.state;
    if (state.kind === 'path') {
        fileManagerFiltersMenu.SetSelectedItem(fileManagerFiltersMenu.GetItemByName('All'));
        fileManager.SetCurrentFolderPath(evt.state.path);
    } else {
        applyFilter(state.filter, state.text);
        fileManagerFiltersMenu.SetSelectedItem(fileManagerFiltersMenu.GetItemByName(state.filter));
    }
};
