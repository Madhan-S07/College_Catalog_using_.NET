// College Management: all the page logic in one file.
//
//   1. Settings
//   2. Small helpers
//   3. Sign in, create account, sign out
//   4. The Add / Edit / Delete engine (used by every table)
//   5. The five tables: Colleges, Departments, Students, Courses, Enrollments
//   6. Reports
//   7. Menu (which page is showing) and start-up

// ---------- 1. Settings ----------

const API = "http://localhost:5230/api";
const REPORT_PAGE_SIZE = 2;   // students per page in the report search

let currentView = "colleges";

// sends the token with every request
$.ajaxSetup({
  beforeSend: function (xhr) {
    const token = localStorage.getItem("token");
    if (token) {
      xhr.setRequestHeader("Authorization", "Bearer " + token);
    }
  }
});

// ---------- 2. Small helpers ----------

// stops the page from running text as HTML
function esc(value) {
  return $("<div>").text(value ?? "").html();
}

// turns a failed request into one readable sentence
function errorText(xhr) {
  if (xhr.status === 0) return "Cannot reach the API. Check that it is running.";
  if (xhr.status === 401) return "Please sign in again. Your session may have expired.";
  if (xhr.status === 403) return "Only an admin can do this.";

  const text = xhr.responseText || "";
  try {
    const body = JSON.parse(text);
    if (body && body.errors) return Object.values(body.errors).flat().join(" ");
    if (body && body.title) return body.title;
  } catch (e) { /* plain text, use it as it is */ }

  return text || "Error " + xhr.status;
}

function clearMessages() {
  $("#message").text("");
  $("#notice").text("");
}

function showError(xhr) {
  $("#notice").text("");
  $("#message").text(errorText(xhr));
}

function showNotice(text) {
  $("#message").text("");
  $("#notice").text(text);
}

// fills a dropdown and keeps the current choice if it is still there
function fillSelect(selector, placeholder, items, valueOf, textOf) {
  const select = $(selector);
  const current = select.val();

  select.empty().append($("<option>").val("").text(placeholder));
  $.each(items, function (i, item) {
    select.append($("<option>").val(valueOf(item)).text(textOf(item)));
  });
  if (current) select.val(current);
}

// loads a list from the API and puts it in a dropdown
function loadOptions(url, selector, placeholder, valueOf, textOf, emptyHint) {
  $.ajax({
    url: url,
    type: "GET",
    success: function (data) {
      fillSelect(selector, placeholder, data, valueOf, textOf);
      if (data.length === 0 && emptyHint) $("#message").text(emptyHint);
    },
    error: showError
  });
}

// shows or hides the "nothing here yet" line under a table
function toggleEmpty(selector, hasRows) {
  $(selector).toggleClass("show", !hasRows);
}

// ---------- 3. Sign in, create account, sign out ----------

const NAME_KEYS = [
  "http://schemas.xmlsoap.org/ws/2005/12/identity/claims/name",
  "unique_name",
  "name"
];
const ROLE_KEYS = [
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
  "role"
];

// reads the username and role from the token, or null if there is no valid token
function readSession() {
  const token = localStorage.getItem("token");
  if (!token) return null;

  try {
    const part = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
    const padded = part + "=".repeat((4 - (part.length % 4)) % 4);
    const bytes = Uint8Array.from(atob(padded), function (c) { return c.charCodeAt(0); });
    const claims = JSON.parse(new TextDecoder().decode(bytes));

    // expired tokens count as signed out
    if (claims.exp && claims.exp * 1000 < Date.now()) return null;

    function pick(keys) {
      for (const key of keys) {
        if (claims[key]) return claims[key];
      }
      return "";
    }
    return { name: pick(NAME_KEYS), role: pick(ROLE_KEYS) };
  } catch (e) {
    return null;
  }
}

function isSignedIn() {
  return readSession() !== null;
}

// updates the sidebar and switches parts of the page on or off (see style.css)
function refreshSession() {
  const session = readSession();
  document.body.dataset.auth = session ? "in" : "out";
  document.body.dataset.role = session ? session.role : "";

  $("#sessionName").text(session ? session.name : "");
  $("#sessionRole").text(session ? session.role : "");
  $("#avatar").text(session && session.name ? session.name.charAt(0).toUpperCase() : "");
}

// LOGIN
$("#loginForm").on("submit", function (e) {
  e.preventDefault();
  $("#authMessage").text("");
  $("#authNotice").text("");

  $.ajax({
    url: API + "/Login/Login",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify({
      user: $("#user").val(),
      password: $("#password").val()
    }),
    dataType: "text",
    success: function (token) {
      localStorage.setItem("token", token);
      $("#loginForm")[0].reset();
      refreshSession();
      showView(currentView);
      showNotice("Signed in.");
    },
    error: function (xhr) { $("#authMessage").text(errorText(xhr)); }
  });
});

// CREATE ACCOUNT
$("#registerForm").on("submit", function (e) {
  e.preventDefault();
  $("#authMessage").text("");
  $("#authNotice").text("");

  $.ajax({
    url: API + "/Login/CreateAccount",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify({
      user: $("#newUser").val(),
      password: $("#newPassword").val()
    }),
    dataType: "text",
    success: function () {
      $("#registerForm")[0].reset();
      $(".tab").first().trigger("click");
      $("#authNotice").text("Account created. Now sign in.");
    },
    error: function (xhr) { $("#authMessage").text(errorText(xhr)); }
  });
});

// sign in / create account tabs
$(".tab").on("click", function () {
  $(".tab").attr("aria-selected", "false");
  $(this).attr("aria-selected", "true");
  $("#loginForm, #registerForm").attr("hidden", true);
  $("#" + $(this).data("target")).removeAttr("hidden");
});

// SIGN OUT
$("#btnSignOut").on("click", function () {
  localStorage.removeItem("token");
  location.hash = "#colleges";
  location.reload();
});

// after every API call: check again that the token is still valid
$(document).ajaxComplete(refreshSession);
setInterval(refreshSession, 30000);

// ---------- 4. The Add / Edit / Delete engine ----------
//
// Every table page has the same parts, so one function builds them all.
// A table only has to say what is different about it (see section 5).
// The page uses these ids, where "x" is the table's prefix (for example "student"):
//   xForm   xTitle   xSave   xCancel   xBody   xEmpty   (and xHint if the table has one)

function setupCrud(c) {
  const p = "#" + c.prefix;
  const getId = c.getId || function (item) { return item.id; };
  const items = {};        // the loaded rows, so Edit can fill the form
  let editingId = null;    // null = adding, otherwise the id being edited

  // puts the form back to "add" mode
  function resetForm() {
    editingId = null;
    $(p + "Form")[0].reset();
    $(p + "Title").text(c.titleAdd);
    $(p + "Save").text(c.saveAdd);
    $(p + "Cancel").prop("hidden", true);
    if (c.hintAdd) $(p + "Hint").text(c.hintAdd);
  }

  // GET the list and show it
  function load() {
    $.ajax({
      url: c.url,
      type: "GET",
      success: function (data) {
        $(p + "Body").empty();
        Object.keys(items).forEach(function (k) { delete items[k]; });

        $.each(data, function (i, item) {
          const id = getId(item);
          items[id] = item;
          $(p + "Body").append(`
            <tr>
              ${c.row(item)}
              <td class="col-action">
                <button class="btnEdit" data-id="${id}">Edit</button>
                <button class="btnDelete" data-id="${id}">Delete</button>
              </td>
            </tr>`);
        });
        toggleEmpty(p + "Empty", data.length > 0);
      },
      error: showError
    });
  }

  // ADD or EDIT, from the same form
  $(p + "Form").on("submit", function (e) {
    e.preventDefault();
    clearMessages();

    const isEdit = editingId !== null;

    $.ajax({
      url: isEdit ? c.url + "/" + editingId : c.url,
      type: isEdit ? "PUT" : "POST",
      contentType: "application/json",
      data: JSON.stringify(c.read()),
      dataType: "text",
      success: function () {
        resetForm();
        load();
        showNotice(isEdit ? c.savedEdit : c.savedAdd);
      },
      error: showError
    });
  });

  // EDIT: copy the row into the form
  $(p + "Body").on("click", ".btnEdit", function () {
    const item = items[$(this).data("id")];
    if (!item) return;

    editingId = getId(item);
    c.fill(item);

    $(p + "Title").text(c.titleEdit);
    $(p + "Save").text(c.saveEdit);
    $(p + "Cancel").prop("hidden", false);
    if (c.hintEdit) $(p + "Hint").text(c.hintEdit);

    clearMessages();
    document.getElementById(c.prefix + "Form").scrollIntoView({ behavior: "smooth", block: "center" });
    $(p + "Form").find("input, select").first().trigger("focus");
  });

  $(p + "Cancel").on("click", function () {
    resetForm();
    clearMessages();
  });

  // DELETE (admin only, the API checks this too)
  $(p + "Body").on("click", ".btnDelete", function () {
    const id = $(this).data("id");
    const item = items[id];

    if (!confirm(c.confirmDelete(item))) return;

    $.ajax({
      url: c.url + "/" + id,
      type: "DELETE",
      dataType: "text",
      success: function () {
        if (editingId === id) resetForm();
        load();
        showNotice(c.savedDelete);
      },
      error: showError
    });
  });

  return { load: load };
}

// ---------- 5. The five tables ----------

// COLLEGES
// Add: if the college and department already exist, the number is ADDED to the count.
// Edit: the number REPLACES the count (use it to correct a wrong number).
const colleges = setupCrud({
  prefix: "college",
  url: API + "/College",
  getId: function (c) { return c.college_Id; },

  titleAdd: "Add a college",
  titleEdit: "Edit college",
  saveAdd: "Add college",
  saveEdit: "Save changes",
  hintAdd: "If the college and department already exist, the number you enter is added to the existing count.",
  hintEdit: "The number you enter replaces the current student count.",
  savedAdd: "College saved. If it already existed, the number was added to its count.",
  savedEdit: "College updated.",
  savedDelete: "College deleted.",

  row: function (c) {
    return `<td>${c.college_Id}</td>
            <td>${esc(c.name)}</td>
            <td>${esc(c.dept)}</td>
            <td>${c.no_Students}</td>`;
  },
  fill: function (c) {
    $("#collegeName").val(c.name);
    $("#collegeDept").val(c.dept);
    $("#collegeCount").val(c.no_Students);
  },
  read: function () {
    return {
      College_Id: 0,
      Name: $("#collegeName").val().trim(),
      Dept: $("#collegeDept").val().trim(),
      No_Students: parseInt($("#collegeCount").val(), 10)
    };
  },
  confirmDelete: function (c) {
    return "Delete " + c.name + " - " + c.dept + "? Its departments, students, courses and enrollments will be deleted too.";
  }
});

// DEPARTMENTS
const departments = setupCrud({
  prefix: "department",
  url: API + "/Department",

  titleAdd: "Add a department",
  titleEdit: "Edit department",
  saveAdd: "Add department",
  saveEdit: "Save changes",
  savedAdd: "Department added.",
  savedEdit: "Department updated.",
  savedDelete: "Department deleted.",

  row: function (d) {
    return `<td>${d.id}</td>
            <td>${esc(d.name)}</td>
            <td>${esc(d.collegeName)}</td>`;
  },
  fill: function (d) {
    $("#departmentName").val(d.name);
    $("#departmentCollege").val(d.collegeId);
  },
  read: function () {
    return {
      name: $("#departmentName").val().trim(),
      collegeId: parseInt($("#departmentCollege").val(), 10)
    };
  },
  confirmDelete: function (d) {
    return "Delete the department " + d.name + "? Its students, courses and enrollments will be deleted too.";
  }
});

// STUDENTS
const students = setupCrud({
  prefix: "student",
  url: API + "/Student",

  titleAdd: "Add a student",
  titleEdit: "Edit student",
  saveAdd: "Add student",
  saveEdit: "Save changes",
  savedAdd: "Student added.",
  savedEdit: "Student updated.",
  savedDelete: "Student deleted.",

  row: function (s) {
    return `<td>${s.id}</td>
            <td>${esc(s.name)}</td>
            <td>${esc(s.email)}</td>
            <td>${esc(s.departmentName)}</td>`;
  },
  fill: function (s) {
    $("#studentName").val(s.name);
    $("#studentEmail").val(s.email);
    $("#studentDept").val(s.departmentId);
  },
  read: function () {
    return {
      name: $("#studentName").val().trim(),
      email: $("#studentEmail").val().trim(),
      departmentId: parseInt($("#studentDept").val(), 10)
    };
  },
  confirmDelete: function (s) {
    return "Delete " + s.name + "? Their enrollments will be deleted too.";
  }
});

// COURSES
const courses = setupCrud({
  prefix: "course",
  url: API + "/Course",

  titleAdd: "Add a course",
  titleEdit: "Edit course",
  saveAdd: "Add course",
  saveEdit: "Save changes",
  savedAdd: "Course added.",
  savedEdit: "Course updated.",
  savedDelete: "Course deleted.",

  row: function (c) {
    return `<td>${c.id}</td>
            <td>${esc(c.title)}</td>
            <td>${c.credits}</td>
            <td>${esc(c.departmentName)}</td>`;
  },
  fill: function (c) {
    $("#courseName").val(c.title);
    $("#courseCredits").val(c.credits);
    $("#courseDept").val(c.departmentId);
  },
  read: function () {
    return {
      title: $("#courseName").val().trim(),
      credits: parseInt($("#courseCredits").val(), 10),
      departmentId: parseInt($("#courseDept").val(), 10)
    };
  },
  confirmDelete: function (c) {
    return "Delete the course " + c.title + "? Its enrollments will be deleted too.";
  }
});

// ENROLLMENTS
const enrollments = setupCrud({
  prefix: "enrollment",
  url: API + "/Enrollment",

  titleAdd: "Enroll a student",
  titleEdit: "Edit enrollment",
  saveAdd: "Enroll student",
  saveEdit: "Save changes",
  savedAdd: "Student enrolled.",
  savedEdit: "Enrollment updated.",
  savedDelete: "Enrollment deleted.",

  row: function (e) {
    return `<td>${e.id}</td>
            <td>${esc(e.studentName)}</td>
            <td>${esc(e.courseTitle)}</td>
            <td>${esc(String(e.enrolledOn).slice(0, 10))}</td>
            <td>${e.grade === null || e.grade === undefined ? "-" : e.grade}</td>`;
  },
  fill: function (e) {
    $("#enrollmentStudent").val(e.studentId);
    $("#enrollmentCourse").val(e.courseId);
    $("#enrollmentGrade").val(e.grade === null || e.grade === undefined ? "" : e.grade);
  },
  read: function () {
    const grade = $("#enrollmentGrade").val();
    return {
      studentId: parseInt($("#enrollmentStudent").val(), 10),
      courseId: parseInt($("#enrollmentCourse").val(), 10),
      grade: grade === "" ? null : parseInt(grade, 10)
    };
  },
  confirmDelete: function (e) {
    return "Remove " + e.studentName + " from " + e.courseTitle + "?";
  }
});

// the dropdowns that each page needs, loaded fresh every time the page opens
function loadCollegeOptions() {
  loadOptions(API + "/College", "#departmentCollege", "Choose a college",
    function (c) { return c.college_Id; },
    function (c) { return c.name + " - " + c.dept; },
    "There are no colleges yet, so a department cannot be added. Add a college first.");
}

function loadDepartmentOptions(selector) {
  loadOptions(API + "/Department", selector, "Choose a department",
    function (d) { return d.id; },
    function (d) { return d.name + " (" + d.collegeName + ")"; },
    "There are no departments yet. Add a department first.");
}

function loadStudentOptions() {
  loadOptions(API + "/Student", "#enrollmentStudent", "Choose a student",
    function (s) { return s.id; },
    function (s) { return s.name + " (" + s.email + ")"; },
    "There are no students yet. Add a student first.");
}

function loadCourseOptions() {
  loadOptions(API + "/Course", "#enrollmentCourse", "Choose a course",
    function (c) { return c.id; },
    function (c) { return c.title + " (" + c.departmentName + ")"; });
}

// ---------- 6. Reports ----------

const reportUrl = API + "/Report";
let reportPage = 1;
let reportTotal = 0;

function avg(value) {
  return value === null || value === undefined ? "-" : value.toFixed(1);
}

// 1. Department summary
function loadSummary() {
  $.ajax({
    url: reportUrl + "/department-summary",
    type: "GET",
    success: function (data) {
      $("#summaryBody").empty();
      $.each(data, function (i, d) {
        $("#summaryBody").append(`
          <tr>
            <td>${esc(d.college)}</td>
            <td>${esc(d.department)}</td>
            <td>${d.studentCount}</td>
            <td>${d.courseCount}</td>
          </tr>`);
      });
      toggleEmpty("#summaryEmpty", data.length > 0);
    },
    error: showError
  });
}

// 2. Top colleges
function loadTopColleges() {
  $.ajax({
    url: reportUrl + "/top-colleges?count=3",
    type: "GET",
    success: function (data) {
      $("#topBody").empty();
      $.each(data, function (i, c) {
        $("#topBody").append(`
          <tr>
            <td>${esc(c.name)}</td>
            <td>${esc(c.dept)}</td>
            <td>${c.studentCount}</td>
          </tr>`);
      });
      toggleEmpty("#topEmpty", data.length > 0);
    },
    error: showError
  });
}

// 3. Course stats
function loadCourseStats() {
  $.ajax({
    url: reportUrl + "/course-stats",
    type: "GET",
    success: function (data) {
      $("#reportCourseBody").empty();
      $.each(data, function (i, c) {
        $("#reportCourseBody").append(`
          <tr>
            <td>${esc(c.title)}</td>
            <td>${esc(c.department)}</td>
            <td>${c.enrolledCount}</td>
            <td>${avg(c.averageGrade)}</td>
          </tr>`);
      });
      toggleEmpty("#reportCourseEmpty", data.length > 0);
    },
    error: showError
  });
}

// 4. Search students with paging
function loadReportStudents() {
  $.ajax({
    url: reportUrl + "/students",
    type: "GET",
    data: { search: $("#reportSearch").val(), page: reportPage, pageSize: REPORT_PAGE_SIZE },
    success: function (result) {
      reportTotal = result.totalCount;
      $("#reportStudentBody").empty();
      $.each(result.items, function (i, s) {
        $("#reportStudentBody").append(`
          <tr>
            <td>${s.id}</td>
            <td>${esc(s.name)}</td>
            <td>${esc(s.email)}</td>
            <td>${esc(s.department)}</td>
            <td><button class="btnView" data-id="${s.id}">View</button></td>
          </tr>`);
      });
      toggleEmpty("#reportStudentEmpty", result.items.length > 0);

      const totalPages = Math.max(1, Math.ceil(reportTotal / REPORT_PAGE_SIZE));
      $("#pageInfo").text("Page " + reportPage + " of " + totalPages + " (" + reportTotal + " students)");
      $("#btnPrev").prop("disabled", reportPage <= 1);
      $("#btnNext").prop("disabled", reportPage >= totalPages);
    },
    error: showError
  });
}

// 5. One student with courses and grades
function loadStudentDetail(id) {
  $.ajax({
    url: reportUrl + "/student/" + id,
    type: "GET",
    success: function (s) {
      let rows = "";
      $.each(s.courses, function (i, c) {
        rows += `<tr>
          <td>${esc(c.title)}</td>
          <td>${c.credits}</td>
          <td>${c.grade === null ? "-" : c.grade}</td>
        </tr>`;
      });

      $("#studentDetail").html(`
        <p><b>${esc(s.name)}</b> (${esc(s.email)})</p>
        <p>Department: ${esc(s.department)} | College: ${esc(s.college)}</p>
        <p>Average grade: ${avg(s.averageGrade)}</p>
        <table>
          <thead><tr><th>Course</th><th>Credits</th><th>Grade</th></tr></thead>
          <tbody>${rows}</tbody>
        </table>`);
    },
    error: showError
  });
}

function loadReports() {
  loadSummary();
  loadTopColleges();
  loadCourseStats();
  loadReportStudents();
}

// buttons
$("#btnReportSearch").on("click", function () {
  reportPage = 1;
  loadReportStudents();
});

$("#btnPrev").on("click", function () {
  reportPage--;
  loadReportStudents();
});

$("#btnNext").on("click", function () {
  reportPage++;
  loadReportStudents();
});

$("#reportStudentBody").on("click", ".btnView", function () {
  loadStudentDetail($(this).data("id"));
});

// ---------- 7. Menu (which page is showing) and start-up ----------

// what to load when each page opens
const views = {
  colleges: function () {
    colleges.load();
  },
  departments: function () {
    loadCollegeOptions();
    departments.load();
  },
  students: function () {
    loadDepartmentOptions("#studentDept");
    students.load();
  },
  courses: function () {
    loadDepartmentOptions("#courseDept");
    courses.load();
  },
  enrollments: function () {
    loadStudentOptions();
    loadCourseOptions();
    enrollments.load();
  },
  reports: loadReports
};

function showView(name) {
  if (!views[name]) name = "colleges";
  currentView = name;

  $(".view").removeClass("active");
  $("#view-" + name).addClass("active");

  $(".nav a").removeAttr("aria-current");
  $('.nav a[data-view="' + name + '"]').attr("aria-current", "page");

  clearMessages();

  // the college list is public, every other page needs a signed-in user
  if (name === "colleges" || isSignedIn()) {
    views[name]();
  }
}

$(window).on("hashchange", function () {
  showView(location.hash.slice(1));
});

// start
$(document).ready(function () {
  refreshSession();
  showView(location.hash.slice(1));
});