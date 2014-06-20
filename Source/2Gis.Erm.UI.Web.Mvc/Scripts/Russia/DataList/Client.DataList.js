function BuildMergeClientsUrl(firstClientId, secondClientId) {
    return String.format('/Russia/ClientsMerging/Merge?masterId={0}&subordinateId={1}', firstClientId, secondClientId);
}