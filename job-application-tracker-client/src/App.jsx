import { useEffect, useMemo, useState } from "react";
import {
  changePassword,
  createApplication,
  createUser,
  deleteApplication,
  getApplicationStats,
  getApplications,
  getUsers,
  login,
  resetUserPassword,
  register,
  updateApplication,
  updateUser,
} from "./api";

const statuses = ["Applied", "Interview", "Offer", "Rejected", "Archived"];
const blankForm = {
  companyName: "",
  position: "",
  status: "Applied",
  dateApplied: new Date().toISOString().slice(0, 10),
  notes: "",
};
const blankAuthSession = { token: "", identifier: "", role: "", requiresPasswordChange: false };
const authBlank = { emailOrUsername: "", password: "" };
const blankAdminUserForm = { identifier: "" };
const blankPasswordForm = { newPassword: "", confirmPassword: "" };

function App() {
  const [authMode, setAuthMode] = useState("login");
  const [authForm, setAuthForm] = useState(authBlank);
  const [authSession, setAuthSession] = useState(() => ({
    token: localStorage.getItem("job-tracker-token") ?? "",
    identifier: localStorage.getItem("job-tracker-identifier") ?? "",
    role: localStorage.getItem("job-tracker-role") ?? "",
    requiresPasswordChange: localStorage.getItem("job-tracker-requires-password-change") === "true",
  }));
  const [applications, setApplications] = useState([]);
  const [users, setUsers] = useState([]);
  const [summaryCounts, setSummaryCounts] = useState({
    activeApplications: 0,
    archivedApplications: 0,
  });
  const [filters, setFilters] = useState({
    companyName: "",
    pageNumber: 1,
    pageSize: 10,
    sortByDateDescending: true,
  });
  const [form, setForm] = useState(blankForm);
  const [editingId, setEditingId] = useState(null);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deletingId, setDeletingId] = useState(null);
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [adminUserForm, setAdminUserForm] = useState(blankAdminUserForm);
  const [editingUserId, setEditingUserId] = useState(null);
  const [tempPasswordNotice, setTempPasswordNotice] = useState("");
  const [passwordForm, setPasswordForm] = useState(blankPasswordForm);
  const isLoggedIn = Boolean(authSession.token);
  const isUser = authSession.role.toLowerCase() === "user";
  const isAdmin = authSession.role.toLowerCase() === "admin";
  const requiresPasswordChange = authSession.requiresPasswordChange && isUser;

  useEffect(() => {
    localStorage.setItem("job-tracker-token", authSession.token);
    localStorage.setItem("job-tracker-identifier", authSession.identifier);
    localStorage.setItem("job-tracker-role", authSession.role);
    localStorage.setItem("job-tracker-requires-password-change", String(authSession.requiresPasswordChange));
  }, [authSession]);

  useEffect(() => {
    if (!isLoggedIn || requiresPasswordChange) {
      return;
    }

    if (isUser) {
      void loadApplications();
      void loadSummaryCounts();
    }
    if (isAdmin) {
      void loadUsers();
    }
  }, [authSession.token, authSession.role, filters.pageNumber, filters.pageSize, filters.sortByDateDescending]);

  async function loadApplications(companyName = filters.companyName) {
    if (!authSession.token) {
      return;
    }

    setLoading(true);
    setError("");

    try {
      const data = await getApplications({ ...filters, companyName }, authSession.token);
      setApplications(data);
    } catch (loadError) {
      setError(loadError.message);
      if (loadError.message.toLowerCase().includes("token")) {
        handleLogout();
      }
    } finally {
      setLoading(false);
    }
  }

  async function loadSummaryCounts() {
    if (!authSession.token || !isUser) {
      setSummaryCounts({ activeApplications: 0, archivedApplications: 0 });
      return;
    }

    try {
      const data = await getApplicationStats(authSession.token);
      setSummaryCounts(data);
    } catch (loadError) {
      setError(loadError.message);
    }
  }

  async function loadUsers() {
    if (!authSession.token || !isAdmin) {
      setUsers([]);
      return;
    }

    setLoading(true);
    setError("");

    try {
      const data = await getUsers(authSession.token);
      setUsers(data);
    } catch (loadError) {
      setError(loadError.message);
      if (loadError.message.toLowerCase().includes("token")) {
        handleLogout();
      }
    } finally {
      setLoading(false);
    }
  }

  async function handleAuthSubmit(event) {
    event.preventDefault();
    setError("");
    setMessage("");

    try {
      const action = authMode === "login" ? login : register;
      const response = await action(authForm);
      setAuthSession({
        token: response.token,
        identifier: response.identifier,
        role: response.role,
        requiresPasswordChange: response.requiresPasswordChange,
      });
      setAuthForm(authBlank);
      setTempPasswordNotice("");
      setMessage(authMode === "login" ? "Welcome back. Your dashboard is ready." : "Account created and signed in.");
    } catch (authError) {
      setError(authError.message);
    }
  }

  async function handleSearchSubmit(event) {
    event.preventDefault();
    const nextFilters = { ...filters, pageNumber: 1 };
    setFilters(nextFilters);
    await loadApplications(nextFilters.companyName);
  }

  async function handleSave(event) {
    event.preventDefault();
    setSaving(true);
    setError("");
    setMessage("");

    try {
      if (editingId) {
        await updateApplication(editingId, form, authSession.token);
        setMessage("Application updated.");
      } else {
        await createApplication(form, authSession.token);
        setMessage("Application added.");
      }

      resetForm();
      await loadApplications();
      await loadSummaryCounts();
    } catch (saveError) {
      setError(saveError.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(id) {
    setDeletingId(id);
    setError("");
    setMessage("");

    try {
      await deleteApplication(id, authSession.token);
      setMessage("Application removed.");

      if (editingId === id) {
        resetForm();
      }

      await loadApplications();
      await loadSummaryCounts();
    } catch (deleteError) {
      setError(deleteError.message);
    } finally {
      setDeletingId(null);
    }
  }

  async function handleAdminUserSave(event) {
    event.preventDefault();
    setSaving(true);
    setError("");
    setMessage("");
    setTempPasswordNotice("");

    try {
      let response;

      if (editingUserId) {
        response = await updateUser(editingUserId, adminUserForm, authSession.token);
      } else {
        response = await createUser(adminUserForm, authSession.token);
      }

      setMessage(response.message);
      setTempPasswordNotice(response.temporaryPassword ? `Temporary password: ${response.temporaryPassword}` : "");
      resetAdminUserForm();
      await loadUsers();
    } catch (saveError) {
      setError(saveError.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleAdminResetPassword() {
    if (!editingUserId) {
      return;
    }

    setSaving(true);
    setError("");
    setMessage("");
    setTempPasswordNotice("");

    try {
      const response = await resetUserPassword(editingUserId, authSession.token);
      setMessage(response.message);
      setTempPasswordNotice(response.temporaryPassword ? `Temporary password: ${response.temporaryPassword}` : "");
      await loadUsers();
    } catch (resetError) {
      setError(resetError.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleChangePassword(event) {
    event.preventDefault();
    setSaving(true);
    setError("");
    setMessage("");

    if (passwordForm.newPassword.length < 6) {
      setError("new password must be at least 6 characters");
      setSaving(false);
      return;
    }

    if (passwordForm.newPassword !== passwordForm.confirmPassword) {
      setError("passwords do not match");
      setSaving(false);
      return;
    }

    try {
      const response = await changePassword({ newPassword: passwordForm.newPassword }, authSession.token);
      setAuthSession((current) => ({ ...current, requiresPasswordChange: false }));
      setPasswordForm(blankPasswordForm);
      setMessage(response.message);
    } catch (changeError) {
      setError(changeError.message);
    } finally {
      setSaving(false);
    }
  }

  function beginEdit(application) {
    setEditingId(application.id);
    setForm({
      companyName: application.companyName,
      position: application.position,
      status: application.status,
      dateApplied: application.dateApplied,
      notes: application.notes ?? "",
    });
    setMessage("Editing selected application.");
    setError("");
  }

  function beginEditUser(user) {
    setEditingUserId(user.id);
    setAdminUserForm({ identifier: user.identifier });
    setTempPasswordNotice("");
    setMessage("Editing selected user.");
    setError("");
  }

  function resetForm() {
    setEditingId(null);
    setForm(blankForm);
  }

  function resetAdminUserForm() {
    setEditingUserId(null);
    setAdminUserForm(blankAdminUserForm);
  }

  function handleLogout() {
    localStorage.removeItem("job-tracker-token");
    localStorage.removeItem("job-tracker-identifier");
    localStorage.removeItem("job-tracker-role");
    setAuthSession(blankAuthSession);
    setApplications([]);
    setUsers([]);
    setSummaryCounts({ activeApplications: 0, archivedApplications: 0 });
    setAdminUserForm(blankAdminUserForm);
    setEditingUserId(null);
    setTempPasswordNotice("");
    setPasswordForm(blankPasswordForm);
    setMessage("Signed out.");
    setError("");
    resetForm();
  }

  const stats = useMemo(() => {
    return applications.reduce(
      (summary, application) => {
        summary.total += 1;
        summary[application.status.toLowerCase()] += 1;
        return summary;
      },
      { total: 0, applied: 0, interview: 0, offer: 0, rejected: 0, archived: 0 },
    );
  }, [applications]);

  return (
    <div className="page-shell">
      <header className="hero-banner">
        <p className="eyebrow">Job Application Tracker</p>
      </header>

      <div className="app-shell">
        <aside className="hero-panel">
          <h1>Job hunt is not easy, tracking helps</h1>
          <p className="hero-copy">
            This application helps you record the details of every job application you submit. Just enter
            information like the company name, position, and notes, and the tracker will keep everything
            organized for you. Add as many applications as you want.
          </p>
          {isLoggedIn && isUser ? (
            <div className="hero-grid">
              <div>
                <strong>{summaryCounts.activeApplications}</strong>
                <span>Active applications</span>
              </div>
              <div>
                <strong>{summaryCounts.archivedApplications}</strong>
                <span>Archived applications</span>
              </div>
            </div>
          ) : null}
        </aside>

        <main className="workspace">
        {!isLoggedIn ? (
          <section className="panel auth-panel">
            <div className="panel-header">
              <div>
                <p className="eyebrow">Secure Access</p>
                <h2>{authMode === "login" ? "Sign in to your tracker" : "Create your account"}</h2>
              </div>
              <button
                className="ghost-button"
                type="button"
                onClick={() => {
                  setAuthMode((current) => (current === "login" ? "register" : "login"));
                  setError("");
                  setMessage("");
                }}
              >
                {authMode === "login" ? "Need an account?" : "Already registered?"}
              </button>
            </div>

            <form className="auth-form" onSubmit={handleAuthSubmit}>
              <label>
                <span>Email / Username</span>
                <input
                  required
                  type="text"
                  value={authForm.emailOrUsername}
                  onChange={(event) => setAuthForm((current) => ({ ...current, emailOrUsername: event.target.value }))}
                />
              </label>

              <label>
                <span>Password</span>
                <input
                  required
                  minLength={6}
                  type="password"
                  value={authForm.password}
                  onChange={(event) => setAuthForm((current) => ({ ...current, password: event.target.value }))}
                />
              </label>

              <button className="primary-button" type="submit">
                {authMode === "login" ? "Sign In" : "Create Account"}
              </button>
            </form>
          </section>
        ) : requiresPasswordChange ? (
          <section className="panel auth-panel">
            <div className="panel-header">
              <div>
                <p className="eyebrow">Password Update</p>
                <h2>Create a new password</h2>
              </div>
            </div>

            <p className="hero-copy auth-copy">
              Your account is using a temporary password. Create a new password before continuing.
            </p>

            <form className="auth-form" onSubmit={handleChangePassword}>
              <label>
                <span>New password</span>
                <input
                  required
                  minLength={6}
                  type="password"
                  value={passwordForm.newPassword}
                  onChange={(event) => setPasswordForm((current) => ({ ...current, newPassword: event.target.value }))}
                />
              </label>

              <label>
                <span>Confirm new password</span>
                <input
                  required
                  minLength={6}
                  type="password"
                  value={passwordForm.confirmPassword}
                  onChange={(event) => setPasswordForm((current) => ({ ...current, confirmPassword: event.target.value }))}
                />
              </label>

              <div className="feedback-stack">
                {message ? <p className="feedback success">{message}</p> : null}
                {error ? <p className="feedback error">{error}</p> : null}
              </div>

              <button className="primary-button" disabled={saving} type="submit">
                {saving ? "Saving..." : "Update Password"}
              </button>
            </form>
          </section>
        ) : (
          <>
            <section className="topbar">
              <div>
                <p className="eyebrow">Dashboard</p>
                <h2>{isAdmin ? "Manage registered users" : "Your pipeline at a glance"}</h2>
                {isAdmin ? (
                  <div className="feedback-stack dashboard-feedback dashboard-feedback--inside">
                    {message ? <p className="feedback success">{message}</p> : null}
                    {error ? <p className="feedback error">{error}</p> : null}
                  </div>
                ) : null}
              </div>
              <button className="ghost-button" type="button" onClick={handleLogout}>
                Sign Out
              </button>
            </section>

            {isUser ? (
              <>
                <section className="stats-row">
                  <StatCard label="Applied" value={stats.applied} accent="applied" />
                  <StatCard label="Interview" value={stats.interview} accent="interview" />
                  <StatCard label="Offer" value={stats.offer} accent="offer" />
                  <StatCard label="Rejected" value={stats.rejected} accent="rejected" />
                </section>

                <section className="workspace-grid">
                  <div className="panel">
                    <div className="panel-header">
                      <div>
                        <p className="eyebrow">Add + Edit</p>
                        <h3>{editingId ? "Update application" : "New application"}</h3>
                      </div>
                      {editingId ? (
                        <button className="ghost-button" type="button" onClick={resetForm}>
                          Cancel edit
                        </button>
                      ) : null}
                    </div>

                    <form className="application-form" onSubmit={handleSave}>
                      <label>
                        <span>Company</span>
                        <input
                          required
                          value={form.companyName}
                          onChange={(event) => setForm((current) => ({ ...current, companyName: event.target.value }))}
                        />
                      </label>

                      <label>
                        <span>Position</span>
                        <input
                          required
                          value={form.position}
                          onChange={(event) => setForm((current) => ({ ...current, position: event.target.value }))}
                        />
                      </label>

                      <div className="form-row">
                        <label>
                          <span>Status</span>
                          <select
                            value={form.status}
                            onChange={(event) => setForm((current) => ({ ...current, status: event.target.value }))}
                          >
                            {statuses.map((status) => (
                              <option key={status} value={status}>
                                {status}
                              </option>
                            ))}
                          </select>
                        </label>

                        <label>
                          <span>Date applied</span>
                          <input
                            required
                            type="date"
                            value={form.dateApplied}
                            onChange={(event) => setForm((current) => ({ ...current, dateApplied: event.target.value }))}
                          />
                        </label>
                      </div>

                      <label>
                        <span>Notes</span>
                        <textarea
                          rows="5"
                          value={form.notes}
                          onChange={(event) => setForm((current) => ({ ...current, notes: event.target.value }))}
                        />
                      </label>

                      <button className="primary-button" disabled={saving} type="submit">
                        {saving ? "Saving..." : editingId ? "Update Application" : "Add Application"}
                      </button>
                    </form>
                  </div>

                  <div className="panel">
                    <div className="panel-header">
                      <div>
                        <p className="eyebrow">Pipeline</p>
                        <h3>Applications</h3>
                      </div>
                    </div>

                    <form className="filters" onSubmit={handleSearchSubmit}>
                      <label className="search-field">
                        <span>Company search</span>
                        <input
                          value={filters.companyName}
                          placeholder="Search by company name"
                          onChange={(event) =>
                            setFilters((current) => ({ ...current, companyName: event.target.value }))
                          }
                        />
                      </label>

                      <label>
                        <span>Page size</span>
                        <select
                          value={filters.pageSize}
                          onChange={(event) =>
                            setFilters((current) => ({
                              ...current,
                              pageSize: Number(event.target.value),
                              pageNumber: 1,
                            }))
                          }
                        >
                          {[5, 10, 20].map((size) => (
                            <option key={size} value={size}>
                              {size}
                            </option>
                          ))}
                        </select>
                      </label>

                      <label className="checkbox-field">
                        <input
                          checked={filters.sortByDateDescending}
                          type="checkbox"
                          onChange={(event) =>
                            setFilters((current) => ({
                              ...current,
                              sortByDateDescending: event.target.checked,
                            }))
                          }
                        />
                        <span>Newest first</span>
                      </label>

                      <button className="secondary-button" type="submit">
                        Apply Filters
                      </button>
                    </form>

                    <div className="pagination-row">
                      <button
                        className="ghost-button"
                        disabled={filters.pageNumber === 1 || loading}
                        type="button"
                        onClick={() =>
                          setFilters((current) => ({ ...current, pageNumber: Math.max(1, current.pageNumber - 1) }))
                        }
                      >
                        Previous
                      </button>
                      <span>Page {filters.pageNumber}</span>
                      <button
                        className="ghost-button"
                        disabled={loading || applications.length < filters.pageSize}
                        type="button"
                        onClick={() =>
                          setFilters((current) => ({ ...current, pageNumber: current.pageNumber + 1 }))
                        }
                      >
                        Next
                      </button>
                    </div>

                    <div className="feedback-stack">
                      {message ? <p className="feedback success">{message}</p> : null}
                      {error ? <p className="feedback error">{error}</p> : null}
                    </div>

                    <div className="application-list">
                      {loading ? <p className="empty-state">Loading applications...</p> : null}
                      {!loading && applications.length === 0 ? (
                        <p className="empty-state">No applications match this view yet.</p>
                      ) : null}

                      {applications.map((application) => (
                        <article className="application-card" key={application.id}>
                          <div className="application-card__header">
                            <div>
                              <p className="card-title">{application.companyName}</p>
                              <h4>{application.position}</h4>
                            </div>
                            <span className={`status-pill status-pill--${application.status.toLowerCase()}`}>
                              {application.status}
                            </span>
                          </div>

                          <p className="application-date">Applied on {application.dateApplied}</p>
                          <p className="application-notes">{application.notes || "No notes added yet."}</p>

                          <div className="card-actions">
                            <button className="secondary-button" type="button" onClick={() => beginEdit(application)}>
                              Edit
                            </button>
                            <button
                              className="danger-button"
                              disabled={deletingId === application.id}
                              type="button"
                              onClick={() => handleDelete(application.id)}
                            >
                              {deletingId === application.id ? "Deleting..." : "Delete"}
                            </button>
                          </div>
                        </article>
                      ))}
                    </div>
                  </div>
                </section>
              </>
            ) : (
              <section className="admin-grid">
                <div className="panel">
                  <div className="panel-header">
                    <div>
                      <p className="eyebrow">Pipeline</p>
                      <h3>Users</h3>
                    </div>
                  </div>

                  <div className="application-list">
                    {loading ? <p className="empty-state">Loading users...</p> : null}
                    {!loading && users.length === 0 ? (
                      <p className="empty-state">No registered users found.</p>
                    ) : null}

                    {users.map((user) => (
                      <article className="application-card" key={user.id}>
                        <div className="application-card__header">
                          <div>
                            <p className="card-title">{user.identifier}</p>
                          </div>
                          <span className={`status-pill status-pill--${user.role.toLowerCase()}${user.requiresPasswordChange ? " status-pill--temp" : ""}`}>
                            {user.requiresPasswordChange ? `${user.role}, temp` : user.role}
                          </span>
                        </div>
                        <div className="card-actions">
                          <button className="secondary-button" type="button" onClick={() => beginEditUser(user)}>
                            Edit
                          </button>
                        </div>
                      </article>
                    ))}
                  </div>
                </div>

                <div className="panel">
                  <div className="panel-header">
                    <div>
                      <p className="eyebrow">Add + Edit</p>
                      <h3>{editingUserId ? "Update user" : "New user"}</h3>
                    </div>
                    {editingUserId ? (
                      <button className="ghost-button" type="button" onClick={resetAdminUserForm}>
                        Cancel edit
                      </button>
                    ) : null}
                  </div>

                  <form className="application-form" onSubmit={handleAdminUserSave}>
                    <label>
                      <span>Email / Username</span>
                      <input
                        required
                        type="text"
                        value={adminUserForm.identifier}
                        onChange={(event) => setAdminUserForm({ identifier: event.target.value })}
                      />
                    </label>

                    <div className="feedback-stack">
                      {message ? <p className="feedback success">{message}</p> : null}
                      {error ? <p className="feedback error">{error}</p> : null}
                      {tempPasswordNotice ? <p className="feedback temp-password">{tempPasswordNotice}</p> : null}
                    </div>

                    <button className="primary-button" disabled={saving} type="submit">
                      {saving ? "Saving..." : editingUserId ? "Update User" : "Create User"}
                    </button>

                    {editingUserId ? (
                      <button
                        className="secondary-button"
                        disabled={saving}
                        type="button"
                        onClick={handleAdminResetPassword}
                      >
                        {saving ? "Working..." : "Force Change Password"}
                      </button>
                    ) : null}
                  </form>
                </div>
              </section>
            )}
          </>
        )}
        </main>
      </div>
    </div>
  );
}

function StatCard({ label, value, accent }) {
  return (
    <article className={`stat-card stat-card--${accent}`}>
      <span>{label}</span>
      <strong>{value}</strong>
    </article>
  );
}

export default App;
