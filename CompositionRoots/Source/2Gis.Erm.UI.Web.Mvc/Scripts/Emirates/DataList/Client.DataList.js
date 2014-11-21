function BuildMergeClientsUrl(firstClientId, secondClientId) {
    return String.format('/Emirates/ClientsMerging/Merge?masterId={0}&subordinateId={1}', firstClientId, secondClientId);
}