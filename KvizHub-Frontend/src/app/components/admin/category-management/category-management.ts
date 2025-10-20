import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../services/admin.service';
import { Category } from '../../../models/user.model';

@Component({
  selector: 'app-category-management',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './category-management.html',
  styleUrl: './category-management.css'
})
export class CategoryManagementComponent implements OnInit {
  categories: Category[] = [];
  isLoading: boolean = true;
  showCreateForm: boolean = false;
  editingCategory: Category | null = null;

  newCategory: Category = {
    id: 0,
    name: '',
    description: ''
  };

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.adminService.getAllCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        this.isLoading = false;
      }
    });
  }

  createCategory(): void {
    if (!this.newCategory.name.trim()) return;

    this.adminService.createCategory(this.newCategory).subscribe({
      next: (category) => {
        this.categories.push(category);
        this.resetForm();
        this.showCreateForm = false;
      },
      error: (error) => {
        console.error('Error creating category:', error);
        alert('Greška pri kreiranju kategorije: ' + error.error?.message);
      }
    });
  }

  startEdit(category: Category): void {
    this.editingCategory = { ...category };
  }

  updateCategory(): void {
    if (!this.editingCategory) return;

    this.adminService.updateCategory(this.editingCategory.id, this.editingCategory).subscribe({
      next: (updatedCategory) => {
        const index = this.categories.findIndex(c => c.id === updatedCategory.id);
        if (index !== -1) {
          this.categories[index] = updatedCategory;
        }
        this.cancelEdit();
      },
      error: (error) => {
        console.error('Error updating category:', error);
        alert('Greška pri ažuriranju kategorije: ' + error.error?.message);
      }
    });
  }

  deleteCategory(categoryId: number): void {
    if (confirm('Da li ste sigurni da želite da obrišete ovu kategoriju?')) {
      this.adminService.deleteCategory(categoryId).subscribe({
        next: () => {
          this.categories = this.categories.filter(c => c.id !== categoryId);
        },
        error: (error) => {
          console.error('Error deleting category:', error);
          alert('Greška pri brisanju kategorije: ' + error.error?.message);
        }
      });
    }
  }

  cancelEdit(): void {
    this.editingCategory = null;
  }

  resetForm(): void {
    this.newCategory = {
      id: 0,
      name: '',
      description: ''
    };
  }
}