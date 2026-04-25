const API_ROOT = import.meta.env.VITE_API_URL?.replace(/\/$/, "") ?? "";

async function request(path, options = {}, token) {
  const response = await fetch(`${API_ROOT}${path}`, {
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers ?? {}),
    },
    ...options,
  });

  if (response.status === 204) {
    return null;
  }

  const data = await response.json().catch(() => null);

  if (!response.ok) {
    const message = data?.message ?? "Something went wrong while talking to the API.";
    throw new Error(message);
  }

  return data;
}

export function register(payload) {
  return request("/api/Auth/register", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export function login(payload) {
  return request("/api/Auth/login", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export function getApplicationStats(token) {
  return request("/api/JobApplications/stats", {}, token);
}

export function getUsers(token) {
  return request("/api/Users", {}, token);
}

export function createUser(payload, token) {
  return request("/api/Users", {
    method: "POST",
    body: JSON.stringify(payload),
  }, token);
}

export function updateUser(id, payload, token) {
  return request(`/api/Users/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  }, token);
}

export function resetUserPassword(id, token) {
  return request(`/api/Users/${id}/reset-password`, {
    method: "POST",
  }, token);
}

export function changePassword(payload, token) {
  return request("/api/Auth/change-password", {
    method: "POST",
    body: JSON.stringify(payload),
  }, token);
}

export function getApplications(query, token) {
  const params = new URLSearchParams({
    pageNumber: String(query.pageNumber),
    pageSize: String(query.pageSize),
    sortByDateDescending: String(query.sortByDateDescending),
  });

  if (query.companyName) {
    params.set("companyName", query.companyName);
  }

  return request(`/api/JobApplications?${params.toString()}`, {}, token);
}

export function createApplication(payload, token) {
  return request("/api/JobApplications", {
    method: "POST",
    body: JSON.stringify(payload),
  }, token);
}

export function updateApplication(id, payload, token) {
  return request(`/api/JobApplications/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  }, token);
}

export function deleteApplication(id, token) {
  return request(`/api/JobApplications/${id}`, {
    method: "DELETE",
  }, token);
}
