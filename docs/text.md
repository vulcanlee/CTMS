.view-container {
    max-width: 1200px;
    margin: 20px auto;
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    grid-gap: 10px;
}

table {
    width: 100%;
    border-collapse: collapse;
}

th, td {
    padding: 8px;
    text-align: left;
    border: 1px solid #ddd;
}

th {
    background-color: #f2f2f2;
}

.left-table {
    background-color: #f8e6f3;
}

.right-table {
    background-color: #e6f3f8;
}

.ct-section {
    display: flex;
    flex-direction: column;
    height: 100%;
}

.ct-header {
    background-color: #9c4f9c;
    color: white;
    text-align: center;
    padding: 15px;
    font-size: 24px;
    font-weight: bold;
}

.ct-image {
    flex-grow: 1;
    background-color: #000;
    display: flex;
    justify-content: center;
    align-items: center;
}

    .ct-image img {
        max-width: 100%;
        max-height: 100%;
    }

.report-section {
    background-color: white;
    padding: 15px;
    text-align: center;
    font-size: 24px;
    font-weight: bold;
}

.nav-buttons {
    display: grid;
    grid-template-columns: repeat(6, 1fr);
    gap: 5px;
    margin-top: 20px;
}

.nav-button {
    padding: 10px;
    text-align: center;
    color: white;
    font-weight: bold;
    cursor: pointer;
}

.btn-clinical {
    background-color: #66bb6a;
}

.btn-image {
    background-color: #9c4f9c;
}

.btn-blood {
    background-color: #29b6f6;
}

.btn-ctcae {
    background-color: #66bb6a;
}

.btn-questionnaire {
    background-color: #29b6f6;
}

.btn-followup {
    background-color: #9c4f9c;
}

.highlight {
    color: red;
    font-weight: bold;
}

.self-filled {
    color: #666;
}

.small-info {
    font-size: 12px;
}
