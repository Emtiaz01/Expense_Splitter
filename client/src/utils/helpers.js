/**
 * Calculate optimized settlements using the greedy algorithm
 * Minimizes the number of transactions needed
 */
export const calculateSettlements = (balances) => {
  // Create copies to avoid modifying original data
  const creditors = balances
    .filter((b) => b.balance > 0)
    .map((b) => ({ ...b }))
    .sort((a, b) => b.balance - a.balance);

  const debtors = balances
    .filter((b) => b.balance < 0)
    .map((b) => ({ ...b, balance: Math.abs(b.balance) }))
    .sort((a, b) => b.balance - a.balance);

  const settlements = [];

  while (creditors.length > 0 && debtors.length > 0) {
    const creditor = creditors[0];
    const debtor = debtors[0];

    const amount = Math.min(creditor.balance, debtor.balance);

    settlements.push({
      from: debtor.userId,
      fromName: debtor.userName,
      to: creditor.userId,
      toName: creditor.userName,
      amount: Number(amount.toFixed(2)),
    });

    creditor.balance -= amount;
    debtor.balance -= amount;

    if (creditor.balance < 0.01) {
      creditors.shift();
    }

    if (debtor.balance < 0.01) {
      debtors.shift();
    }
  }

  return settlements;
};

/**
 * Format currency
 */
export const formatCurrency = (amount) => {
  return new Intl.NumberFormat("bn-BD", {
    style: "currency",
    currency: "BDT",
  }).format(amount);
};

/**
 * Format date
 */
export const formatDate = (date) => {
  return new Date(date).toLocaleDateString("en-BD", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
};

/**
 * Calculate split amounts based on split type
 */
export const calculateSplits = (
  amount,
  participants,
  splitType,
  customSplits = null,
) => {
  const splits = [];

  switch (splitType) {
    case "Equal":
      const equalShare = amount / participants.length;
      participants.forEach((userId) => {
        splits.push({
          userId,
          shareAmount: Number(equalShare.toFixed(2)),
          percentage: null,
        });
      });
      break;

    case "Unequal":
      if (customSplits) {
        customSplits.forEach((split) => {
          splits.push({
            userId: split.userId,
            shareAmount: Number(split.shareAmount),
            percentage: null,
          });
        });
      }
      break;

    case "Percentage":
      if (customSplits) {
        customSplits.forEach((split) => {
          const shareAmount = (amount * split.percentage) / 100;
          splits.push({
            userId: split.userId,
            shareAmount: Number(shareAmount.toFixed(2)),
            percentage: split.percentage,
          });
        });
      }
      break;

    default:
      break;
  }

  return splits;
};

/**
 * Validate splits
 */
export const validateSplits = (amount, splits, splitType) => {
  if (!splits || splits.length === 0) {
    return { valid: false, message: "At least one participant is required" };
  }

  if (splitType === "Unequal") {
    const total = splits.reduce(
      (sum, split) => sum + Number(split.shareAmount || 0),
      0,
    );
    if (Math.abs(total - amount) > 0.01) {
      return {
        valid: false,
        message: `Split amounts must equal ${formatCurrency(amount)}. Current total: ${formatCurrency(total)}`,
      };
    }
  }

  if (splitType === "Percentage") {
    const totalPercentage = splits.reduce(
      (sum, split) => sum + Number(split.percentage || 0),
      0,
    );
    if (Math.abs(totalPercentage - 100) > 0.01) {
      return {
        valid: false,
        message: `Percentages must equal 100%. Current total: ${totalPercentage}%`,
      };
    }
  }

  return { valid: true };
};
