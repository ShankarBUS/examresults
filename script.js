// Dummy data for testing purposes
const dummyData = {
    "resultcode": "200",
    "resultmessage": "Success",
    "result": {
        "footer_message": "The Statement of Marks bears no correction",
        "student": [
            {
                "registration_no": "520020104xxx",
                "student_name": "Big Shungus",
                "course": "BACHELOR OF MEDICINE & BACHELOR OF SURGERY",
                "institution_name": "Government Medical College",
                "exam_session_name": "Mar 2025",
                "regulation": "2019-2020 (NON-SEMESTER)",
                "term_name": "THIRD PROFESSIONAL PART-II",
                "result_publish_from_date": "01-05-2025",
                "subject": [
                    {
                        "subject_code": "526081,\n526082",
                        "subject_name": "GENERAL MEDICINE (PAPER I & II)",
                        "paper": [
                            { "paper_name": "THEORY I", "obtained_mark": "50" },
                            { "paper_name": "THEORY II", "obtained_mark": "50" },
                            { "paper_name": "THEORY TOTAL IN MARKS", "obtained_mark": "100" },
                            { "paper_name": "PRACTICAL/CLINICAL + VIVA IN MARKS", "obtained_mark": "150" },
                            { "paper_name": "TOTAL (THEORY+PRACTICAL/CLINICAL+VIVA) IN %", "obtained_mark": "82.5" }
                        ],
                        "result": "PASS"
                    },
                    {
                        "subject_code": "526083,\n526084",
                        "subject_name": "GENERAL SURGERY (PAPER I & II)",
                        "paper": [
                            { "paper_name": "THEORY I", "obtained_mark": "48" },
                            { "paper_name": "THEORY II", "obtained_mark": "52" },
                            { "paper_name": "THEORY TOTAL IN MARKS", "obtained_mark": "100" },
                            { "paper_name": "PRACTICAL/CLINICAL + VIVA IN MARKS", "obtained_mark": "100" },
                            { "paper_name": "TOTAL (THEORY+PRACTICAL/CLINICAL+VIVA) IN %", "obtained_mark": "75" }
                        ],
                        "result": "PASS"
                    },
                    {
                        "subject_code": "526085,\n526086",
                        "subject_name": "OBSTETRICS & GYNAECOLOGY (PAPER I & II)",
                        "paper": [
                            { "paper_name": "THEORY I", "obtained_mark": "56" },
                            { "paper_name": "THEORY II", "obtained_mark": "46" },
                            { "paper_name": "THEORY TOTAL IN MARKS", "obtained_mark": "102" },
                            { "paper_name": "PRACTICAL/CLINICAL + VIVA IN MARKS", "obtained_mark": "120" },
                            { "paper_name": "TOTAL (THEORY+PRACTICAL/CLINICAL+VIVA) IN %", "obtained_mark": "85.5" }
                        ],
                        "result": "PASS"
                    },
                    {
                        "subject_code": "526087",
                        "subject_name": "PEDIATRICS",
                        "paper": [
                            { "paper_name": "THEORY IN MARKS", "obtained_mark": "44" },
                            { "paper_name": "PRACTICAL/CLINICAL + VIVA IN MARKS", "obtained_mark": "60" },
                            { "paper_name": "TOTAL (THEORY+PRACTICAL/CLINICAL+VIVA) IN %", "obtained_mark": "52" }
                        ],
                        "result": "PASS"
                    }
                ]
            }
        ]
    }
};

async function showResults() {
    const regNo = document.getElementById('regNo').value;
    if (!regNo) {
        alert('Please enter a registration number.');
        return;
    }

    const apiUrl = `https://cms2api.tnmgrmu.ac.in/Api/index.php/StudentPreview/previewGradeMarkAllCourse?registration_no=${regNo}&term_code=THIRD%20PROFESSIONAL%20PART-II`;
    console.log('API URL:', apiUrl);
    try {
        const response = await fetch(apiUrl);
        if (!response.ok) {
            throw new Error('Failed to fetch results.');
        }

        const data = await response.json();
        if (data.resultcode !== '200') {
            throw new Error('No results found.');
        }
        displayResults(data);
    } catch (error) {
        const useDummyData = confirm('API call failed. Do you want to use dummy data?');
        if (useDummyData) {
            displayResults(dummyData);
        }
    }
}

document.getElementById('showResults').addEventListener('click', showResults);

document.getElementById('regNo').addEventListener('keypress', (event) => {
    if (event.key === 'Enter') showResults();
});

function isPass(result) {
    return result.toLowerCase() === 'pass';
}

function displayResults(data) {
    const container = document.getElementById('resultsContainer');
    container.innerHTML = '';

    if (data.resultcode !== '200') {
        container.innerHTML = '<p>No results found.</p>';
        return;
    }

    const student = data.result.student[0];

    const card = document.createElement('div');
    card.className = 'card';

    const infoTable = document.createElement('table');
    infoTable.className = 'info-table';

    const addRow = (label, value) => {
        const row = document.createElement('tr');

        const labelCell = document.createElement('td');
        labelCell.textContent = label;
        labelCell.className = 'label-cell';

        const valueCell = document.createElement('td');
        valueCell.textContent = value;
        valueCell.className = 'value-cell';

        row.appendChild(labelCell);
        row.appendChild(valueCell);
        infoTable.appendChild(row);
    };

    addRow('Name', student.student_name);
    addRow('Registration Number', student.registration_no);
    addRow('Course', student.course);
    addRow('Institution', student.institution_name);
    addRow('Exam Session', student.exam_session_name);
    addRow('Regulation', student.regulation);
    addRow('Term', student.term_name);
    addRow('Result Publish Date', student.result_publish_from_date);

    card.appendChild(infoTable);

    const allpass = student.subject.every(subject => isPass(subject.result));
    const failedSubjects = student.subject.filter(subject => !isPass(subject.result));
    const failedSubjectsText = failedSubjects.map(subject => subject.subject_name).join(', ');

    if (allpass) {
        const passMessage = document.createElement('p');
        passMessage.textContent = 'Congratulations! You have passed all subjects.';
        passMessage.className = 'pass-message';
        card.appendChild(passMessage);
    } else {
        const failMessage = document.createElement('p');
        failMessage.textContent = `You have failed in the following subjects: ${failedSubjectsText}`;
        failMessage.className = 'fail-message';
        card.appendChild(failMessage);
    }

    const accordion = document.createElement('div');
    accordion.className = 'accordion-grid';

    student.subject.forEach(subject => {
        const item = document.createElement('div');
        item.className = 'accordion-item';

        const button = document.createElement('button');
        button.className = 'accordion-button';

        const subjectText = document.createElement('span');
        subjectText.textContent = `${subject.subject_name}`;
        subjectText.className = 'subject-text';
        button.appendChild(subjectText);

        const badgeArea = document.createElement('div');
        badgeArea.className = 'badge-area';

        button.appendChild(badgeArea);        

        const badge = document.createElement('span');
        badge.textContent = subject.result;
        badge.className = `badge ${isPass(subject.result) ? 'badge-pass' : 'badge-fail'}`;

        const totalPercentage = parseFloat(subject.paper.find(paper => paper.paper_name == 'TOTAL (THEORY+PRACTICAL/CLINICAL+VIVA) IN %').obtained_mark);
        if (totalPercentage >= 75) {
            const specialBadge = document.createElement('span');
            if (totalPercentage >= 80) {
                specialBadge.textContent = 'H';
                specialBadge.className = 'badge badge-honors';
            } else if (totalPercentage >= 75) {
                specialBadge.textContent = 'D';
                specialBadge.className = 'badge badge-distinction';
            }
            badgeArea.appendChild(specialBadge);
        }

        badgeArea.appendChild(badge);

        button.addEventListener('click', () => {
            content.style.display = content.style.display === 'block' ? 'none' : 'block';
        });
        item.appendChild(button);

        const content = document.createElement('div');
        content.className = 'accordion-item-content';

        subject.paper.forEach(paper => {
            const paperInfo = document.createElement('p');
            paperInfo.textContent = `${paper.paper_name}: ${paper.obtained_mark}`;
            content.appendChild(paperInfo);
        });

        item.appendChild(content);
        accordion.appendChild(item);
    });

    card.appendChild(accordion);
    container.appendChild(card);

    document.body.classList.add('results-fetched');
}
