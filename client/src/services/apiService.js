import api from "./api";

export const authService = {
  async register(userData) {
    const response = await api.post("/auth/register", userData);
    return response.data;
  },

  async login(credentials) {
    const response = await api.post("/auth/login", credentials);
    return response.data;
  },

  async getCurrentUser() {
    const response = await api.get("/auth/me");
    return response.data;
  },

  async searchUserByEmail(email) {
    const response = await api.get(
      `/auth/search?email=${encodeURIComponent(email)}`,
    );
    return response.data;
  },
};

export const groupService = {
  async getGroups() {
    const response = await api.get("/groups");
    return response.data;
  },

  async getGroup(groupId) {
    const response = await api.get(`/groups/${groupId}`);
    return response.data;
  },

  async createGroup(groupData) {
    const response = await api.post("/groups", groupData);
    return response.data;
  },

  async addMember(groupId, userId) {
    const response = await api.post(`/groups/${groupId}/members`, { userId });
    return response.data;
  },

  async searchAndAddMember(groupId, email) {
    // First search for the user
    const user = await authService.searchUserByEmail(email);
    // Then add them to the group
    const response = await api.post(`/groups/${groupId}/members`, {
      userId: user.userId,
    });
    return response.data;
  },

  async removeMember(groupId, memberId) {
    const response = await api.delete(`/groups/${groupId}/members/${memberId}`);
    return response.data;
  },

  async closeGroup(groupId) {
    const response = await api.put(`/groups/${groupId}/close`);
    return response.data;
  },
};

export const expenseService = {
  async getExpenses(groupId) {
    const response = await api.get(`/groups/${groupId}/expenses`);
    return response.data;
  },

  async createExpense(expenseData) {
    const response = await api.post("/expenses", expenseData);
    return response.data;
  },

  async updateExpense(expenseId, expenseData) {
    const response = await api.put(`/expenses/${expenseId}`, expenseData);
    return response.data;
  },

  async deleteExpense(expenseId) {
    const response = await api.delete(`/expenses/${expenseId}`);
    return response.data;
  },
};

export const balanceService = {
  async getBalances(groupId) {
    const response = await api.get(`/groups/${groupId}/balances`);
    return response.data;
  },

  async getSettlements(groupId) {
    const response = await api.get(`/groups/${groupId}/settlements`);
    return response.data;
  },

  async createSettlement(groupId, settlementData) {
    const response = await api.post(
      `/groups/${groupId}/settle`,
      settlementData,
    );
    return response.data;
  },
};
