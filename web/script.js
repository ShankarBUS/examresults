import { setupMessagePopup, showMessagePopup } from 'https://shankarbus.github.io/kaadu-ui/kaadu-ui.js';

setupMessagePopup();

// Fetching and displaying the results based on the entered registration number.
async function fetchResultsFromRegNo(regNo) {
  const loginUrl = `https://cms2api.tnmgrmu.ac.in/Api/index.php/Login/appLogin?registration_no=${regNo}&login_type=result`;
  const loadCourseUrlBase = `https://cms2api.tnmgrmu.ac.in/Api/index.php/Login/loadCourseTerm?registration_no=${regNo}&exam_session=`;
  const resultUrlBase = `https://cms2api.tnmgrmu.ac.in/Api/index.php/StudentPreview/previewGradeMarkAllCourse?registration_no=${regNo}&term_code=`;

  // Step 1: Get exam_session
  const loginResponse = await fetch(loginUrl);
  if (!loginResponse.ok) {
    throw new Error('FAILED TO FETCH EXAM SESSION.');
  }
  const loginData = await loginResponse.json();
  if (loginData.resultcode !== '200') {
    throw new Error('INVALID REGISTRATION NUMBER.');
  }
  const examSession = loginData.result.exam_session;
  if (!examSession) {
    throw new Error('EXAM SESSION NOT FOUND.');
  }

  // Step 2: Get course_term
  const loadCourseUrl = `${loadCourseUrlBase}${encodeURIComponent(examSession)}`;
  const courseResponse = await fetch(loadCourseUrl);
  if (!courseResponse.ok) {
    throw new Error('FAILED TO FETCH COURSE TERM.');
  }
  const courseData = await courseResponse.json();
  if (courseData.resultcode !== '200' || courseData.result.length === 0) {
    throw new Error('INVALID REGISTRATION NUMBER.');
  }
  // The student may have written one or more exams recently, but we take only the first one.
  const termCode = courseData.result[0].course_term;
  if (!termCode) {
    throw new Error('COURSE TERM NOT FOUND.');
  }

  // Step 3: Get result data
  const resultUrl = `${resultUrlBase}${encodeURIComponent(termCode)}`;
  const resultResponse = await fetch(resultUrl);
  if (!resultResponse.ok) {
    throw new Error('FAILED TO FETCH RESULTS.');
  }
  const resultData = await resultResponse.json();
  if (resultData.resultcode !== '200') {
    throw new Error('NO RESULTS FOUND.');
  }
  return resultData;
}

async function showResults() {
  const regNo = document.getElementById('regNo').value;
  if (!regNo) {
    showMessagePopup('PLEASE ENTER A REGISTRATION NUMBER.');
    return;
  }

  const loadingProgressBar = document.getElementById('loadingProgressBar');
  loadingProgressBar.style.display = 'block';

  try {
    const resultData = await fetchResultsFromRegNo(regNo);
    displayResults(resultData);
  } catch (error) {
    showMessagePopup(error.message);
    // const useDummyData = confirm('API CALL FAILED. DO YOU WANT TO USE DUMMY DATA?');
    // if (useDummyData) {
    //   const dummyData = fetch(`./mockdata/dummy${regNo}.json`)
    //     .then(response => response.json())
    //     .then(data => {
    //       displayResults(data);
    //     })
    //     .catch((e) => {
    //       showMessagePopup(e.message);
    //       //displayResults(backupDummyData);
    //     });
    // }
  } finally {
    loadingProgressBar.style.display = 'none';
  }
}

document.getElementById('showResults').addEventListener('click', showResults);

document.getElementById('regNo').addEventListener('keypress', (event) => {
  if (event.key === 'Enter') showResults();
});

document.getElementById('backButton').addEventListener('click', () => {
  document.getElementById('resultsContainer').innerHTML = '';
  document.getElementById('regNo').value = '';
  document.body.classList.remove('results-fetched');
});

function isPass(result) {
  return result.toLowerCase() === 'pass';
}

function displayResults(data) {
  const container = document.getElementById('resultsContainer');
  container.innerHTML = '';

  if (!data || !data.result || data.resultcode !== '200'
    || !data.result.student || data.result.student.length === 0) {
    container.innerHTML = '<p>NO RESULTS FOUND.</p>';
    return;
  }

  const student = data.result.student[0];

  const group = document.createElement('div');
  group.className = 'group result-group';

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

  addRow('NAME:', student.student_name);
  addRow('REGISTRATION NUMBER:', student.registration_no);
  addRow('COURSE:', student.course);
  addRow('INSTITUTION:', student.institution_name);
  addRow('EXAM SESSION:', student.exam_session_name);
  addRow('REGULATION:', student.regulation);
  addRow('TERM:', student.term_name);
  addRow('RESULT PUBLISH DATE:', student.result_publish_from_date);

  group.appendChild(infoTable);

  const allpass = student.subject.every(subject => isPass(subject.result));
  const failedSubjects = student.subject.filter(subject => !isPass(subject.result));
  const failedSubjectsText = failedSubjects.map(subject => subject.subject_name).join(', ');

  if (allpass) {
    const passMessage = document.createElement('p');
    passMessage.textContent = 'CONGRATULATIONS! YOU HAVE PASSED ALL SUBJECTS.';
    passMessage.className = 'pass-message';
    group.appendChild(passMessage);
  } else {
    const failMessage = document.createElement('p');
    failMessage.textContent = `YOU HAVE FAILED IN THE FOLLOWING SUBJECTS: ${failedSubjectsText}`;
    failMessage.className = 'fail-message';
    group.appendChild(failMessage);
  }

  const accordion = document.createElement('div');
  accordion.className = 'accordion-grid';

  student.subject.forEach(subject => {
    const item = document.createElement('div');
    item.className = 'accordion-item';

    const button = document.createElement('button');
    button.className = `accordion-button ${isPass(subject.result) ? '' : 'accordion-button-fail'}`;

    const detailsPreview = document.createElement('div');
    detailsPreview.className = 'details-preview';
    button.appendChild(detailsPreview);

    const subjectText = document.createElement('span');
    subjectText.textContent = `${subject.subject_name}`;
    subjectText.className = 'subject-text';
    detailsPreview.appendChild(subjectText);

    const badgeArea = document.createElement('div');
    badgeArea.className = 'badge-area';

    detailsPreview.appendChild(badgeArea);

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

    const totalText = document.createElement('span');
    totalText.textContent = `${totalPercentage.toFixed(1)}%`;
    totalText.className = 'total-text';
    button.appendChild(totalText);

    button.addEventListener('click', () => {
      content.style.display = content.style.display === 'block' ? 'none' : 'block';
    });
    item.appendChild(button);

    const content = document.createElement('div');
    content.className = 'accordion-item-content';

    const markTable = document.createElement('table');
    markTable.className = 'info-table';
    subject.paper.forEach(paper => {
      const row = document.createElement('tr');

      const paperNameCell = document.createElement('td');
      paperNameCell.className = 'label-cell';
      paperNameCell.textContent = paper.paper_name;
      const obtainedMarkCell = document.createElement('td');
      obtainedMarkCell.className = 'value-cell';
      obtainedMarkCell.textContent = paper.obtained_mark;

      row.appendChild(paperNameCell);
      row.appendChild(obtainedMarkCell);
      markTable.appendChild(row);
    });

    content.appendChild(markTable);
    item.appendChild(content);
    accordion.appendChild(item);
  });

  group.appendChild(accordion);
  container.appendChild(group);

  document.body.classList.add('results-fetched');
}

const disclaimerButton = document.getElementById('disclaimerButton');

disclaimerButton.addEventListener('click', () => {
  showMessagePopup('DISCLAIMER: THIS WEBSITE IS NOT AFFILIATED WITH TNMGRMU. IT IS AN UNOFFICIAL PLATFORM FOR VIEWING EXAM RESULTS, PROVIDED FOR INFORMATIONAL PURPOSES ONLY. FOR OFFICIAL AND AUTHENTIC RESULTS, PLEASE VISIT THE TNMGRMU OFFICIAL WEBSITE.');
});
